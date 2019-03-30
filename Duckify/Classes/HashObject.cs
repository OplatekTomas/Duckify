using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Duckify.Classes {
    public class HashObject {

        public static string GenerateKey(Object sourceObject) {
            string hashString;

            //Catch unuseful parameter values
            if (sourceObject == null) {
                throw new ArgumentNullException("Null as parameter is not allowed");
            } else {
                //Now we begin to do the real work.
                hashString = ComputeHash(ObjectToByteArray(sourceObject));
                return hashString;           
            }
        }

        private static string ComputeHash(byte[] objectAsBytes) {
            MD5 md5 = new MD5CryptoServiceProvider();
            try {
                byte[] result = md5.ComputeHash(objectAsBytes);

                // Build the final string by converting each byte
                // into hex and appending it to a StringBuilder
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++) {
                    sb.Append(result[i].ToString("X2"));
                }

                // And return it
                return sb.ToString();
            } catch (ArgumentNullException ane) {
                //If something occurred during serialization, 
                //this method is called with a null argument. 
                Console.WriteLine("Hash has not been generated.");
                return null;
            }
        }

        private static readonly object locker = new object();

        private static byte[] ObjectToByteArray(object objectToSerialize) {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try {
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
                lock (locker) {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            } catch (SerializationException se) {
                Console.WriteLine("Error occurred during serialization. Message: " +
                se.Message);
                return null;
            } finally {
                fs.Close();
            }
        }
    }
}
