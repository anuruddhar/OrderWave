#region Modification Log
/*------------------------------------------------------------------------------------------------------------------------------------------------- 
   System      -   OrderWave
   Module      -   Core

Modification History:
==================================================================================================================================================
Date              Version      		Modify by              					Description
--------------------------------------------------------------------------------------------------------------------------------------------------
31/05/2015         	  1.0           Anuruddha   					        Initial Version
--------------------------------------------------------------------------------------------------------------------------------------------------*/
#endregion
#region Namespace
using OrderWave.Core.UserInfo;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace OrderWave.Core.Security {


    public class Encrypter {
        private const string KEY_ID = "!@#$%^&*()AX826Gwkf58e?s";
        private const int SALTLENTH = 4;
        private const string VECTOR_ID = "w48*+-36dfthjklo";
        //Public Function Encrypt(ByVal plainText As String) As String
        //End Function

        // create salted password to save in Db
        public byte[] CreateDBPassword(byte[] unsaltedPassword) {
            //Create a salt value
            var saltValue = new byte[SALTLENTH];

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltValue);

            return CreateSaltedPassword(saltValue, unsaltedPassword);
        }

        // create a salted password given the salt value
        private byte[] CreateSaltedPassword(byte[] saltValue, byte[] unsaltedPassword) {
            // add the salt to the hash
            var rawSalted = new byte[unsaltedPassword.Length + saltValue.Length];
            unsaltedPassword.CopyTo(rawSalted, 0);
            saltValue.CopyTo(rawSalted, unsaltedPassword.Length);

            //Create the salted hash			
            var sha1 = SHA1.Create();
            var saltedPassword = sha1.ComputeHash(rawSalted);

            // add the salt value to the salted hash
            var dbPassword = new byte[saltedPassword.Length + saltValue.Length];
            saltedPassword.CopyTo(dbPassword, 0);
            saltValue.CopyTo(dbPassword, saltedPassword.Length);

            return dbPassword;
        }

        public static string Decrypt(string cipherText) {
            var memStreamDecryptedData = new MemoryStream();

            var rij = default(Rijndael);
            rij = new RijndaelManaged();
            rij.Mode = CipherMode.CBC;

            var transform = rij.CreateDecryptor(Encoding.ASCII.GetBytes(KEY_ID), Encoding.ASCII.GetBytes(VECTOR_ID));
            var decStream = new CryptoStream(memStreamDecryptedData, transform, CryptoStreamMode.Write);
            var bytesData = Convert.FromBase64String(cipherText);
            try {
                decStream.Write(bytesData, 0, bytesData.Length);
            } catch (Exception ex) {
                throw new Exception("Error while writing encrypted data to the stream:" + ex.Message);
            }
            decStream.FlushFinalBlock();
            decStream.Close();

            return Encoding.ASCII.GetString(memStreamDecryptedData.ToArray());
        }



        public static string Encrypt(string planText) {
            var memStreamEncryptedData = new MemoryStream();
            var rij = default(Rijndael);
            rij = new RijndaelManaged();
            rij.Mode = CipherMode.CBC;

            rij.Key = Encoding.ASCII.GetBytes(KEY_ID);
            rij.IV = Encoding.ASCII.GetBytes(VECTOR_ID);

            var bytesData = Encoding.ASCII.GetBytes(planText);

            var transform = rij.CreateEncryptor();
            var encStream = new CryptoStream(memStreamEncryptedData, transform, CryptoStreamMode.Write);

            try {
                encStream.Write(bytesData, 0, bytesData.Length);
            } catch (Exception ex) {
                throw new Exception("Error while writing encrypted data to the stream:" + ex.Message);
            }

            encStream.FlushFinalBlock();
            encStream.Close();

            return Convert.ToBase64String(memStreamEncryptedData.ToArray());
        }

        public static string GetDALConnection(string connectionString) {
            User user = new User() {
                UserId = UserHelper.GetUsernameWithDomain(),
                Workstation = UserHelper.GetWorkstation()
            };
            var planConnectionString = Decrypt(connectionString);
            //var workstationID = string.Empty;
            string UserAndWorkstation = string.Empty;
            if (string.IsNullOrEmpty(user.UserId))
                UserAndWorkstation = user.Workstation.Trim();
            else
                UserAndWorkstation = string.Concat(user.Workstation.Trim().ToUpper(), "|", user.UserId.Trim().ToUpper());

            planConnectionString = string.Format(planConnectionString, UserAndWorkstation);
            return planConnectionString;
        }

        public static string GetDALConnection(string connectionString, string workstationID) {
            var planConnectionString = Decrypt(connectionString);

            planConnectionString = string.Format(planConnectionString, workstationID.Trim().ToUpper());
            return planConnectionString;
        }
    }

    /*
    public class Encrypter {

        private const int SALTLENTH = 4;
        private const string KEY_ID = "!@#$%^&*()AX826Gwkf58e?s";
        private const string VECTOR_ID = "w48*+-36dfthjklo";

        // compare the contents of two byte arrays 
        private bool CompareByteArray (byte[] array1, byte[] array2) {
            if ((array1.Length != array2.Length)) {
                return false;
            } int i = 0;
            for (i = 0; i <= array1.Length - 1; i++) {
                if ((array1[i] != array2[i])) {
                    return false;
                }
            } return true;
        }

        public static string Encrypt (string planText) {
            MemoryStream memStreamEncryptedData = new MemoryStream();
            Rijndael rij = default(Rijndael);
            rij = new RijndaelManaged();
            rij.Mode = CipherMode.CBC;
            rij.Key = Encoding.ASCII.GetBytes(KEY_ID);
            rij.IV = Encoding.ASCII.GetBytes(VECTOR_ID);
            byte[] bytesData = Encoding.ASCII.GetBytes(planText);
            ICryptoTransform transform = rij.CreateEncryptor();
            CryptoStream encStream = new CryptoStream(memStreamEncryptedData, transform, CryptoStreamMode.Write);
            try {
                encStream.Write(bytesData, 0, bytesData.Length);
            } catch (Exception ex) {
                throw new Exception("Error while writing encrypted data to the stream:" + ex.Message);
            }
            encStream.FlushFinalBlock();
            encStream.Close();
            return Convert.ToBase64String(memStreamEncryptedData.ToArray());
        }

        public static string Decrypt (string cipherText) {
            MemoryStream memStreamDecryptedData = new MemoryStream();
            Rijndael rij = default(Rijndael);
            rij = new RijndaelManaged();
            rij.Mode = CipherMode.CBC;
            ICryptoTransform transform = rij.CreateDecryptor(Encoding.ASCII.GetBytes(KEY_ID), Encoding.ASCII.GetBytes(VECTOR_ID));
            CryptoStream decStream = new CryptoStream(memStreamDecryptedData, transform, CryptoStreamMode.Write);
            byte[] bytesData = Convert.FromBase64String(cipherText);
            try {
                decStream.Write(bytesData, 0, bytesData.Length);
            } catch (Exception ex) {
                throw new Exception("Error while writing encrypted data to the stream:" + ex.Message);
            }
            decStream.FlushFinalBlock();
            decStream.Close();
            return Encoding.ASCII.GetString(memStreamDecryptedData.ToArray());
        }

    }*/

}
