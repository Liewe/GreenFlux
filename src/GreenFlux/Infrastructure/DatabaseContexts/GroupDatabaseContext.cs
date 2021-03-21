using System.Collections.Generic;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DatabaseContexts
{
    public interface IGroupDatabaseContext
    {
        void Initialize();
        Group GetByGroupIdentifier(string groupIdentifier);
        IEnumerable<Group> GetAll();
        int Save(Group group);
        int DeleteByGroupIdentifier(string groupIdentifier);
    }

    public class GroupDatabaseContext : IGroupDatabaseContext
    {

        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "Groups";

        public GroupDatabaseContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(Group.Identifier)}] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
                    [{nameof(Group.Name)}] TEXT NOT NULL,
                    [{nameof(Group.CapacityInAmps)}] INTEGER NOT NULL   
                );

                CREATE UNIQUE INDEX IF NOT EXISTS [PK_{TableName}] 
                ON {TableName}([{nameof(Group.Identifier)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }
        
        public Group GetByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(Group.Identifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.QuerySingleOrDefault<Group>(sql, new { groupIdentifier });
        }

        public IEnumerable<Group> GetAll()
        {
            var sql = $@"SELECT * FROM [{TableName}]";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Query<Group>(sql);
        }

        public int Save(Group group)
        {
            var sql = $@"           
                INSERT INTO [{TableName}](
                    [{nameof(Group.Identifier)}],
                    [{nameof(Group.Name)}],
                    [{nameof(Group.CapacityInAmps)}])
                VALUES(
                    @{nameof(Group.Identifier)},
                    @{nameof(Group.Name)},
                    @{nameof(Group.CapacityInAmps)})
                ON CONFLICT([{nameof(Group.Identifier)}])
                DO UPDATE SET 
                    [{nameof(Group.Name)}] = @{nameof(Group.Name)},
                    [{nameof(Group.CapacityInAmps)}] = @{nameof(Group.CapacityInAmps)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, group);
        }

        public int DeleteByGroupIdentifier(string groupIdentifier)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Group.Identifier)}] = @{nameof(groupIdentifier)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupIdentifier });
        }
    }
}
