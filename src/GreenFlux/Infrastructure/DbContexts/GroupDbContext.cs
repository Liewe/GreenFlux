using System.Collections.Generic;
using Dapper;
using GreenFlux.Infrastructure.Models;

namespace GreenFlux.Infrastructure.DbContexts
{
    public interface IGroupDbContext
    {
        void Initialize();
        Group GetByGroupId(string groupId);
        IEnumerable<Group> GetAll();
        int Save(Group group);
        int DeleteByGroupId(string groupId);
    }

    public class GroupDbContext : IGroupDbContext
    {

        private readonly IConnectionFactory _connectionFactory;

        public const string TableName = "Groups";

        public GroupDbContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Initialize()
        {
            var sql = $@"
                CREATE TABLE IF NOT EXISTS [{TableName}] 
                (
                    [{nameof(Group.Id)}] UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
                    [{nameof(Group.Name)}] TEXT NOT NULL,
                    [{nameof(Group.CapacityInAmps)}] INTEGER NOT NULL   
                );

                CREATE UNIQUE INDEX IF NOT EXISTS [PK_{TableName}] 
                ON {TableName}([{nameof(Group.Id)}]);";

            using var connection = _connectionFactory.GetDbConnection();
            connection.Execute(sql);
        }
        
        public Group GetByGroupId(string groupId)
        {
            var sql = $@"
                SELECT * FROM [{TableName}]
                WHERE [{nameof(Group.Id)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.QuerySingleOrDefault<Group>(sql, new { groupId });
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
                    [{nameof(Group.Id)}],
                    [{nameof(Group.Name)}],
                    [{nameof(Group.CapacityInAmps)}])
                VALUES(
                    @{nameof(Group.Id)},
                    @{nameof(Group.Name)},
                    @{nameof(Group.CapacityInAmps)})
                ON CONFLICT([{nameof(Group.Id)}])
                DO UPDATE SET 
                    [{nameof(Group.Name)}] = @{nameof(Group.Name)},
                    [{nameof(Group.CapacityInAmps)}] = @{nameof(Group.CapacityInAmps)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, group);
        }

        public int DeleteByGroupId(string groupId)
        {
            var sql = $@"           
                DELETE FROM [{TableName}]
                WHERE [{nameof(Group.Id)}] = @{nameof(groupId)}";

            using var connection = _connectionFactory.GetDbConnection();
            return connection.Execute(sql, new { groupId });
        }
    }
}
