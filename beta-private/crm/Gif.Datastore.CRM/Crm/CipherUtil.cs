using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Samc4.CipherUtil
{
  public static class CipherUtil
  {
    public static string Decrypt<T>(string text, string password, string salt) where T : SymmetricAlgorithm, new()
    {
      string end;
      DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
      SymmetricAlgorithm symmetricAlgorithm = Activator.CreateInstance<T>();
      byte[] bytes = rfc2898DeriveByte.GetBytes(symmetricAlgorithm.KeySize >> 3);
      byte[] numArray = rfc2898DeriveByte.GetBytes(symmetricAlgorithm.BlockSize >> 3);
      ICryptoTransform cryptoTransform = symmetricAlgorithm.CreateDecryptor(bytes, numArray);
      using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(text)))
      {
        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
        {
          using (StreamReader streamReader = new StreamReader(cryptoStream, Encoding.Unicode))
          {
            end = streamReader.ReadToEnd();
          }
        }
      }
      return end;
    }
  }
}
