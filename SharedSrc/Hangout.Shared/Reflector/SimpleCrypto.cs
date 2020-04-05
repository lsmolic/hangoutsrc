using System.IO;

namespace Hangout.Shared
{
    public class SimpleCrypto
    {
        //example key and vector
        //private byte[] mTdesKey = { 00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
        //private byte[] mTdesIV = { 255, 254, 253, 252, 251, 250, 249, 248 };
        //private TripleDESCryptoServiceProvider mTripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();

        public MemoryStream TDesEncrypt(MemoryStream packet)
        {
			return packet;
			//byte[] bytes = new byte[packet.Length];
			//packet.Read(bytes, 0, (int) packet.Length);
			//byte[] encryptedBytes = Transform(bytes, mTripleDESCryptoServiceProvider.CreateEncryptor(mTdesKey, mTdesIV));
			//MemoryStream encryptedPacket = ConvertByteArrayToMemoryStream(encryptedBytes);
			//return encryptedPacket;

        }
        public string TDesEncrypt(string plainText)
        {
            return plainText;
            //byte[] bytes = System.Text.Encoding.Unicode.GetBytes(plainText);
            //byte[] encryptedBytes = Transform(bytes, mTripleDESCryptoServiceProvider.CreateEncryptor(mTdesKey, mTdesIV));
            //return HexStringFromByteArray(encryptedBytes);
        }

        public MemoryStream TDesDecrypt(MemoryStream packet)
        {
			return packet;

			//TripleDESCryptoServiceProvider m_tDes = new TripleDESCryptoServiceProvider();

			//byte[] bytes = new byte[packet.Length];
			//packet.Read(bytes, 0, (int)packet.Length);
			//byte[] decryptedBytes = Transform(bytes, mTripleDESCryptoServiceProvider.CreateDecryptor(mTdesKey, mTdesIV));
			//MemoryStream decryptedPacket = ConvertByteArrayToMemoryStream(decryptedBytes);

			//return decryptedPacket;
        }
        
        public string TDesDecrypt(string cipherText)
        {
            return cipherText;
            //byte[] bytes = HexStringToByteArray(cipherText);
            //byte[] decryptedBytes = Transform(bytes, mTripleDESCryptoServiceProvider.CreateDecryptor(mTdesKey, mTdesIV));
            //string plaintext = System.Text.Encoding.Unicode.GetString(decryptedBytes);
            //return plaintext;
        }
        /*
        private byte[] Transform(byte[] input, ICryptoTransform cryptoTransform)
        {
            // Create the necessary streams

            MemoryStream memory = new MemoryStream();
            CryptoStream stream;
            try
            {
                stream = new CryptoStream(memory, cryptoTransform, CryptoStreamMode.Write);
            }
            catch (ArgumentException argumentEx)
            {
                throw argumentEx;
            }
            // Transform the bytes as requested
            try
            {
                stream.Write(input, 0, input.Length);
            }
            catch (NotSupportedException notSupportedEx)
            {
                throw notSupportedEx;
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeEx)
            {
                throw argumentOutOfRangeEx;
            }
            catch (ArgumentException argumentEx)
            {
                throw argumentEx;
            }

            try
            {
                stream.FlushFinalBlock();
            }
            catch (CryptographicException cryptographicEx)
            {
                throw cryptographicEx;
            }
            catch (NotSupportedException notSupportedEx)
            {
                throw notSupportedEx;
            }

            // Read the memory stream and convert it back into byte array
            memory.Position = 0;
            byte[] result = new byte[memory.Length];
            try
            {
                memory.Read(result, 0, result.Length);
            }
            catch (ArgumentNullException argumentNullEx)
            {
                throw argumentNullEx;
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeEx)
            {
                throw argumentOutOfRangeEx;
            }
            catch (ArgumentException argumentEx)
            {
                throw argumentEx;
            }
            catch (ObjectDisposedException objectDisposedEx)
            {
                throw objectDisposedEx;
            }
            // Clean up

            memory.Close(); 
            stream.Close();


            return result;
        }

        private MemoryStream ConvertByteArrayToMemoryStream(byte[] input)
        {
            MemoryStream resultStream = new MemoryStream();

            try
            {
                resultStream.Write(input, 0, input.Length);
            }
            catch (ArgumentNullException argumentNullEx)
            {
                throw argumentNullEx;
            }
            catch (NotSupportedException notSupportedEx)
            {
                throw notSupportedEx;
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeEx)
            {
                throw argumentOutOfRangeEx;
            }
            catch (ArgumentException argumentEx)
            {
                throw argumentEx;
            }
            catch (IOException systemIOEx)
            {
                throw systemIOEx;
            }
            catch (ObjectDisposedException objectDisposedEx)
            {
                throw objectDisposedEx;
            }

            resultStream.Position = 0;

            return resultStream;
        }

        private string HexStringFromByteArray(byte[] bytes) {
            string s = "";

            foreach (byte b in bytes) {
                s += string.Format("{0:X2}", b);
            }

            return s;
        }
        private byte[] HexStringToByteArray(string hexString) {

            string newString = "";
            char c;
            // remove all none A-F, 0-9, characters
            for (int i = 0; i < hexString.Length; i++) {
                c = hexString[i];
                if (IsHexDigit(c))
                {
                    newString += c;
                }
            }
            // if odd number of characters, discard last character
            if (newString.Length % 2 != 0) {
                newString = newString.Substring(0, newString.Length - 1);
            }

            int byteLength = newString.Length / 2;
            byte[] bytes = new byte[byteLength];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++) {
                hex = new String(new Char[] { newString[j], newString[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }
        private bool IsHexDigit(Char c) {
            int numChar;
            int numA = Convert.ToInt32('A');
            int num1 = Convert.ToInt32('0');
            c = Char.ToUpper(c);
            numChar = Convert.ToInt32(c);
            if (numChar >= numA && numChar < (numA + 6))
            {
                return true;
            }
            if (numChar >= num1 && numChar < (num1 + 10))
            {
                return true;
            }
            return false;
        }
        private byte HexToByte(string hex) {
            byte newByte = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return newByte;
        }
        */
    }
}
