using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace LightningQueues.Storage
{
    public interface IColumn
    {
        Session Session { get; set; }
        Table Table { get; set; }
        IDictionary<string, JET_COLUMNID> Columns { get; set; }
    }

    public interface IColumn<T> : IColumn
    {
        void Set(string columnName, T value);
        T Get(string columnName);
    }

    public class DateTimeColumn : ColumnBase, IColumn<DateTime>
    {
        public void Set(string columnName, DateTime value)
        {
            Api.SetColumn(Session, Table, Columns[columnName], value.ToOADate());
        }

        public DateTime Get(string columnName)
        {
            return DateTime.FromOADate(Api.RetrieveColumnAsDouble(Session, Table, Columns[columnName]).Value);
        }

        public DateTime? GetOrDefault(string columnName)
        {
            var value = Api.RetrieveColumnAsDouble(Session, Table, Columns[columnName]);
            if (value == null)
                return null;
            return DateTime.FromOADate(value.Value);
        }
    }

    public abstract class ColumnBase : IColumn
    {
        public Session Session { get; set; }
        public Table Table { get; set; }
        public IDictionary<string, JET_COLUMNID> Columns { get; set; }
    }

    public class GuidColumn : ColumnBase, IColumn<Guid>
    {
        public void Set(string columnName, Guid value)
        {
            Api.SetColumn(Session, Table, Columns[columnName], value.ToByteArray());
        }

        public Guid Get(string columnName)
        {
            return new Guid(Api.RetrieveColumn(Session, Table, Columns[columnName]));
        }
    }

    public class StringColumn : ColumnBase, IColumn<string>
    {
        public void Set(string columnName, string value)
        {
            Api.SetColumn(Session, Table, Columns[columnName], value, Encoding.Unicode);
        }

        public string Get(string columnName)
        {
            return Api.RetrieveColumnAsString(Session, Table, Columns[columnName], Encoding.Unicode);
        }
    }

    public class IntColumn : ColumnBase, IColumn<int>
    {
        public void Set(string columnName, int value)
        {
            Api.SetColumn(Session, Table, Columns[columnName], value);
        }

        public void InterlockedIncrement(string columnName, int increment)
        {
            var bytes = BitConverter.GetBytes(increment);
            int actual;
            Api.JetEscrowUpdate(Session, Table, Columns[columnName], bytes, bytes.Length, null, 0, out actual, EscrowUpdateGrbit.None);
        }

        public int InterlockedRead(string columnName)
        {
    		var bytes = new byte[4];
    		var zero = BitConverter.GetBytes(0);
    		int actual;
    		Api.JetEscrowUpdate(Session, Table, Columns[columnName],
    		                    zero, zero.Length, bytes, bytes.Length, out actual, EscrowUpdateGrbit.None);
    		return BitConverter.ToInt32(bytes, 0);
        }

        public int Get(string columnName)
        {
            return Api.RetrieveColumnAsInt32(Session, Table, Columns[columnName]).Value;
        }

        public int? GetOrDefault(string columnName)
        {
            return Api.RetrieveColumnAsInt32(Session, Table, Columns[columnName]);
        }
    }

    public class BytesColumn : ColumnBase, IColumn<byte[]>
    {
        public void Set(string columnName, byte[] value)
        {
            Api.SetColumn(Session, Table, Columns[columnName], value);
        }

        public byte[] Get(string columnName)
        {
            return Api.RetrieveColumn(Session, Table, Columns[columnName]);
        }
    }
}