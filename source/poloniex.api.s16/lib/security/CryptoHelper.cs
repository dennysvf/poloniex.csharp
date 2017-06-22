using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Poloniex.LIB.Security
{
    //-----------------------------------------------------------------------------------------------------------------------------
    //
    //-----------------------------------------------------------------------------------------------------------------------------
    /// <summary></summary>
    public class CCryptoHelper
    {
        //-----------------------------------------------------------------------------------------------------------------------------
        // 
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public CCryptoHelper()
        {
            Cryptors = new List<CCryption>();
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        // 
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        private static List<CCryption> Cryptors
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_crypto_key"></param>
        /// <returns></returns>
        private static CCryption GetCryptor(string p_crypto_key = "")
        {
            p_crypto_key = CCryption.IsExistKey(p_crypto_key) == false ? CCryption.DefaultKey : p_crypto_key;

            CCryption _cryptor = Cryptors.Find
                (
                    delegate(CCryption p_cryptor)
                    {
                        return p_cryptor.SelectedKey == p_crypto_key;
                    }
                );

            if (_cryptor == null)
            {
                _cryptor = new Poloniex.LIB.Security.CCryption(p_crypto_key);
                Cryptors.Add(_cryptor);
            }

            return _cryptor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] CreateNewCryptoKey()
        {
            string[] _result = new string[2];

            using (var _rijndael = Aes.Create())
            {
                _result[0] = Convert.ToBase64String(_rijndael.Key);
                _result[1] = Convert.ToBase64String(_rijndael.IV);
            }

            return _result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        // 
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_plain_text"></param>
        /// <param name="p_cryptor_key"></param>
        /// <param name="p_encoding"></param>
        /// <returns></returns>
        public static string PlainToChiper(string p_plain_text, string p_cryptor_key = "", Encoding p_encoding = null)
        {
            var _result = p_plain_text;

            if (String.IsNullOrEmpty(p_plain_text) == false)
                _result = GetCryptor(p_cryptor_key).PlainToChiper(p_plain_text, p_encoding);

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_chiper_text"></param>
        /// <param name="p_cryptor_key"></param>
        /// <param name="p_encoding"></param>
        /// <returns></returns>
        public static string ChiperToPlain(string p_chiper_text, string p_cryptor_key = "", Encoding p_encoding = null)
        {
            var _result = p_chiper_text;

            if (String.IsNullOrEmpty(p_chiper_text) == false)
                _result = GetCryptor(p_cryptor_key).ChiperToPlain(p_chiper_text, p_encoding);

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_plain_text"></param>
        /// <param name="p_compress"></param>
        /// <param name="p_cryptor_key"></param>
        /// <returns></returns>
        public static string PlainToChiperText(string p_plain_text, bool p_compress = false, string p_cryptor_key = "")
        {
            var _result = p_plain_text;

            if (String.IsNullOrEmpty(p_plain_text) == false)
                _result = GetCryptor(p_cryptor_key).PlainToChiperText(p_plain_text, p_compress);

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_chiper_text"></param>
        /// <param name="p_compress"></param>
        /// <param name="p_cryptor_key"></param>
        /// <returns></returns>
        public static string ChiperTextToPlain(string p_chiper_text, bool p_compress = false, string p_cryptor_key = "")
        {
            var _result = p_chiper_text;

            if (String.IsNullOrEmpty(p_chiper_text) == false)
                _result = GetCryptor(p_cryptor_key).ChiperTextToPlain(p_chiper_text, p_compress);

            return _result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_object"></param>
        /// <param name="p_compress"></param>
        /// <param name="p_cryptor_key"></param>
        /// <returns></returns> 
        public static byte[] PlainToChiperBytes(object p_object, bool p_compress = false, string p_cryptor_key = "")
        {
            return GetCryptor(p_cryptor_key).PlainToChiperBytes(p_object, p_compress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_chiper_bytes"></param>
        /// <param name="p_compress"></param>
        /// <param name="p_cryptor_key"></param>
        /// <returns></returns>
        public static object ChiperBytesToPlain(byte[] p_chiper_bytes, bool p_compress = false, string p_cryptor_key = "")
        {
            return GetCryptor(p_cryptor_key).ChiperBytesToPlain(p_chiper_bytes, p_compress);
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        //
        //-----------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_source"></param>
        /// <returns></returns>
        public static string MD5ComputeHash(byte[] p_source)
        {
            return BitConverter.ToString(MD5.Create().ComputeHash(p_source)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_source"></param>
        /// <returns></returns>
        public static string SHA1ComputeHash(byte[] p_source)
        {
            return BitConverter.ToString(SHA1.Create().ComputeHash(p_source)).Replace("-", "").ToLower();
        }

        /// <summary>
        /// compute hash value of the file
        /// </summary>
        /// <param name="p_file_name"></param>
        /// <returns></returns>
        public static string ComputeFileHash(string p_file_name)
        {
            var _result = Guid.NewGuid().ToString();

            using (var _fs = new FileStream(p_file_name, FileMode.Open, FileAccess.Read, FileShare.Read, 4096))
            {
                using (var _crypto = SHA1.Create())
                {
                    byte[] _hashs = _crypto.ComputeHash(_fs);
                    _result = BitConverter.ToString(_hashs);
                }
            }

            return _result;
        }

        //-----------------------------------------------------------------------------------------------------------------------------
        // 
        //-----------------------------------------------------------------------------------------------------------------------------
    }
}
