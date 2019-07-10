using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBreeze;
using DBreeze.Transactions;
using DBreeze.Utils;

namespace HyperSpaceDB.Engine.Managers
{
    public class HyperSpaceDBManager
    {
        static HyperSpaceDBManager _instance;
        DBreezeEngine _engine;
        ConcurrentDictionary<Guid, Transaction> _transactions;
        public static HyperSpaceDBManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("You should call CreateInstance first");

                return _instance;
            }
        }
        public static void CreateInstance(string dbPath)
        {
            _instance = new HyperSpaceDBManager(dbPath);
        }
        public HyperSpaceDBManager(string dsbPath)
        {
            _transactions = new ConcurrentDictionary<Guid, Transaction>();
            _engine = new DBreezeEngine(dsbPath);

        }

        public Guid CreateTransaction()
        {
            Guid key = Guid.NewGuid();
            var transaction = _engine.GetTransaction();
            _transactions[key] = transaction;

            return key;
        }

        public Guid CreateTransaction(Guid transactionKey)
        {
            var transaction = _engine.GetTransaction();
            _transactions[transactionKey] = transaction;

            return transactionKey;
        }

        public void CommitTransaction(Guid transactionKey)
        {
            _transactions[transactionKey].Commit();
            _transactions[transactionKey].Dispose();
            //_transactions.TryRemove(transactionKey, out r);
        }

        public void RollbackTransaction(Guid transactionKey)
        {
            _transactions[transactionKey].Rollback();
            _transactions[transactionKey].Dispose();
        }

        public void Insert(Guid transactionKey, byte[] obj)
        {

            byte[] key = null;
            key = 1.ToIndex(_transactions[transactionKey].ObjectGetNewIdentity<long>("Propositions"), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01), new DateTime(2019, 01, 01));
            _transactions[transactionKey].Insert<byte[], byte[]>("Propositions", key, obj);
        }

        public ulong Count(string table)
        {
            using (var tran = _engine.GetTransaction())
            {
                ulong cnt = tran.Count(table);
                return cnt;
            }
        }
    }
}
