using System.Collections.Generic;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
	public class ColumnsInformation
	{
		public IDictionary<string, JET_COLUMNID> RecoveryColumns;
		public IDictionary<string, JET_COLUMNID> OutgoingColumns;
		public IDictionary<string, JET_COLUMNID> SubqueuesColumns;
		public IDictionary<string, JET_COLUMNID> OutgoingHistoryColumns;
		public IDictionary<string, JET_COLUMNID> RecveivedMsgsColumns;
		public IDictionary<string, JET_COLUMNID> TxsColumns;
		public IDictionary<string, JET_COLUMNID> QueuesColumns;
	}
}