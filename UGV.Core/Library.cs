using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UGV.Core
{
    #region AES

    /// <summary>
    /// AES Encryotor and Decryptor
    /// </summary>
    public class AES
    {
        /// <summary>
        /// AES Encryptor
        /// </summary>
        private ICryptoTransform Encryptor;

        /// <summary>
        /// AES Edcryptor
        /// </summary>
        private ICryptoTransform Decryptor;

        /// <summary>
        /// Constructor of AES
        /// </summary>
        /// <param name="Key"></param>
        public AES(string Key)
        {
            //create key from share key
            byte[] key = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(Key));
            byte[] iv = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Key));
            //create aes
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.Key = key;
            aes.IV = iv;
            Encryptor = aes.CreateEncryptor();
            Decryptor = aes.CreateDecryptor();
        }

        /// <summary>
        /// Encrypt and a plain tex
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] input)
        {
            //encrypt
            return Encryptor.TransformFinalBlock(input, 0, input.Length);
        }

        /// <summary>
        /// Decrypt and a plain tex
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] input)
        {
            //decrypt
            return Decryptor.TransformFinalBlock(input, 0, input.Length);
        }
    }

    #endregion AES

    public static class Serialize
    {
        #region XML Serializer

        /// <summary>
        /// XML Serialization
        /// </summary>
        public static string XmlSerialize<T>(T t)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.Serialize(ms, t);
            string xmlString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            return xmlString;
        }
        /// <summary>
        /// XML Deserialization
        /// </summary>
        public static T XmlDeserialize<T>(string XmlString)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XmlString));
            T obj = (T)ser.Deserialize(ms);
            return obj;
        }

        #endregion XML
    }
}
