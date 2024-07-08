using System.Reflection;
using Npgsql;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

internal class Program

{

    public static X509Certificate2 GetCombinedCertificateAndKey(string certificatePath, string privateKeyPath)
    {
        using var publicKey = new X509Certificate2(certificatePath);

        var privateKeyText = System.IO.File.ReadAllText(privateKeyPath);
        var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
        var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
        using var rsa = RSA.Create();

        if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
        {
            rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        }
        else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
        {
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
        }

        var keyPair = publicKey.CopyWithPrivateKey(rsa);
        var Certificate = new X509Certificate2(keyPair.Export(X509ContentType.Pfx));
        return Certificate;
    }

    public static void MyClientCertificates(X509CertificateCollection certificates)
    {
        // TODO: change this
        X509Certificate2 clientCert = GetCombinedCertificateAndKey("/host_data/acra-client.crt", "/host_data/acra-client.key");

        certificates.Add(clientCert);
    }

    static void Main(string[] args)
    {
        var connectionString = "User ID=" + "test"
               + ";Password=" + "test"
               + ";Server=" + "acra-server"
               + ";Port=" + "9393"
               + ";Database=" + "test"
               + ";Integrated Security=true;Pooling=true;SSL Mode=Require;Trust Server Certificate=true";

        var connection = new NpgsqlConnection(connectionString);
        connection.ProvideClientCertificatesCallback += new ProvideClientCertificatesCallback(MyClientCertificates);

        for (int i = 0; i < 1000000; i++)
        {
            try
            {
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("open");
                    string query = "SELECT * from customer;";
                    using var command = new NpgsqlCommand(query, connection);
                    using var reader = command.ExecuteReader();

                    // while (reader.Read())
                    // {
                    //     Console.WriteLine(reader["name"]);

                    // }
                }
                else
                {
                    Console.WriteLine("failed to connect");
                }
                connection.Close();

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }

}