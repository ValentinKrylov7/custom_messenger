#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

using BestHTTP.PlatformSupport.Memory;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Macs;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Tls
{
    /// <summary>
    /// A generic TLS MAC implementation, acting as an HMAC based on some underlying Digest.
    /// </summary>
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.NullChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.ArrayBoundsChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.DivideByZeroChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class TlsMac
    {
        private readonly TlsContext context;
        private readonly byte[] secret;
        private readonly IMac mac;
        private readonly int digestBlockSize;
        private readonly int digestOverhead;
        private readonly int macLength;

        /**
         * Generate a new instance of an TlsMac.
         *
         * @param context the TLS client context
         * @param digest  The digest to use.
         * @param key     A byte-array where the key for this MAC is located.
         * @param keyOff  The number of bytes to skip, before the key starts in the buffer.
         * @param keyLen  The length of the key.
         */
        public TlsMac(TlsContext context, IDigest digest, byte[] key, int keyOff, int keyLen)
        {
            this.context = context;

            KeyParameter keyParameter = new KeyParameter(key, keyOff, keyLen);

            this.secret = Arrays.Clone(keyParameter.GetKey());

            // TODO This should check the actual algorithm, not rely on the engine type
            if (digest is LongDigest)
            {
                this.digestBlockSize = 128;
                this.digestOverhead = 16;
            }
            else
            {
                this.digestBlockSize = 64;
                this.digestOverhead = 8;
            }

            if (TlsUtilities.IsSsl(context))
            {
                this.mac = new Ssl3Mac(digest);

                // TODO This should check the actual algorithm, not assume based on the digest size
                if (digest.GetDigestSize() == 20)
                {
                    /*
                     * NOTE: When SHA-1 is used with the SSL 3.0 MAC, the secret + input pad is not
                     * digest block-aligned.
                     */
                    this.digestOverhead = 4;
                }
            }
            else
            {
                this.mac = new HMac(digest);

                // NOTE: The input pad for HMAC is always a full digest block
            }

            this.mac.Init(keyParameter);

            this.macLength = mac.GetMacSize();
            if (context.SecurityParameters.truncatedHMac)
            {
                this.macLength = System.Math.Min(this.macLength, 10);
            }
        }

        /**
         * @return the MAC write secret
         */
        public /*virtual */byte[] MacSecret
        {
            get { return this.secret; }
        }

        /**
         * @return The output length of this MAC.
         */
        public /*virtual */int Size
        {
            get { return macLength; }
        }

        /**
         * Calculate the MAC for some given data.
         *
         * @param type    The message type of the message.
         * @param message A byte-buffer containing the message.
         * @param offset  The number of bytes to skip, before the message starts.
         * @param length  The length of the message.
         * @return A new byte-buffer containing the MAC value.
         */
        public /*virtual */BufferSegment CalculateMac(long seqNo, byte type, byte[] message, int offset, int length)
        {
            ProtocolVersion serverVersion = context.ServerVersion;
            bool isSsl = serverVersion.IsSsl;

            int macHeaderLength = isSsl ? 11 : 13;
            byte[] macHeader = BufferPool.Get(macHeaderLength, true);
            TlsUtilities.WriteUint64(seqNo, macHeader, 0);
            TlsUtilities.WriteUint8(type, macHeader, 8);
            if (!isSsl)
            {
                TlsUtilities.WriteVersion(serverVersion, macHeader, 9);
            }
            TlsUtilities.WriteUint16(length, macHeader, macHeaderLength - 2);

            mac.BlockUpdate(macHeader, 0, macHeaderLength);
            mac.BlockUpdate(message, offset, length);

            BufferPool.Release(macHeader);

            return Truncate(MacUtilities.DoFinalOptimized(mac));
        }

        public /*virtual */BufferSegment CalculateMacConstantTime(long seqNo, byte type, byte[] message, int offset, int length, int fullLength, byte[] dummyData)
        {
            /*
             * Actual MAC only calculated on 'length' bytes...
             */
            BufferSegment result = CalculateMac(seqNo, type, message, offset, length);

            /*
             * ...but ensure a constant number of complete digest blocks are processed (as many as would
             * be needed for 'fullLength' bytes of input).
             */
            int headerLength = TlsUtilities.IsSsl(context) ? 11 : 13;

            // How many extra full blocks do we need to calculate?
            int extra = GetDigestBlockCount(headerLength + fullLength) - GetDigestBlockCount(headerLength + length);

            while (--extra >= 0)
            {
                mac.BlockUpdate(dummyData, 0, digestBlockSize);
            }

            // One more byte in case the implementation is "lazy" about processing blocks
            mac.Update(dummyData[0]);
            mac.Reset();

            return result;
        }

        protected /*virtual */int GetDigestBlockCount(int inputLength)
        {
            // NOTE: This calculation assumes a minimum of 1 pad byte
            return (inputLength + digestOverhead) / digestBlockSize;
        }

        protected /*virtual */BufferSegment Truncate(BufferSegment bs)
        {
            if (bs.Count <= macLength)
            {
                return bs;
            }

            return new BufferSegment(bs.Data, bs.Offset, macLength); //Arrays.CopyOf(bs, macLength);
        }
    }
}
#pragma warning restore
#endif
