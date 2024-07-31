using System.Data;
using System.Security.Cryptography.X509Certificates;
using Npgsql;
using Dapper;
using member.api.Shared.Models;
using member.api.Shared.Services;

namespace member.api.Shared.Repositories
{
    public abstract class BaseRepository : IBaseRepository
    {
        protected readonly string _connectionString;
        protected readonly IDbConnection _connection;
        protected readonly bool _sslMode;
        protected readonly Certificate _certs;
        private readonly ISecurityEncryption _securityEncryption;
        private readonly bool _decrypt;
        protected BaseRepository(string connectionString, bool sslMode, Certificate certs)
        {
            _connectionString = connectionString;
            _sslMode = sslMode;
            _certs = certs;
        }

        protected BaseRepository
        (
            string connectionString,
            bool sslMode,
            Certificate certs,
            ISecurityEncryption securityEncryption,
            bool? decrypt
        )
        {
            _connectionString = connectionString;
            _sslMode = sslMode;
            _certs = certs;
            _decrypt = decrypt ?? false;
            _securityEncryption = securityEncryption;
        }


        protected BaseRepository(IDbConnection connection) => _connection = connection;

        public virtual IDbConnection OpenDbConnection()
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;

            string connectionString = _connectionString;

            if (!string.IsNullOrEmpty(connectionString))
            {

                if (_sslMode)
                {
                    var connection = new NpgsqlConnection(connectionString);
                    connection.ProvideClientCertificatesCallback += _ProvoideCert;
                    return connection;
                }
                else
                {
                    return new NpgsqlConnection(connectionString);
                }

            }
            return _connection;
        }
        public virtual IDbConnection GetDbConnection()
        {
            return OpenDbConnection();
        }
        private void _ProvoideCert(X509CertificateCollection clientCerts)
        {
            var caPath = new X509Certificate2(_certs.ServerCa);
            var ca = new X509Certificate2(caPath);
            clientCerts.Add(ca);
        }
    }
}