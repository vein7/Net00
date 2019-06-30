using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfficeOpenXml;

namespace Wisdom4s.Web.Entity
{
    public interface IReadCell
    {
        void Reading(Msg msg, object obj);
    }

    public interface ISheetReader
    {
        int StartRow { get; }
        string GetMsg();
        bool BeforeRead(ExcelRange range, ExcelCellAddress end);
        void ReadRange();
        void ReadFinished();
    }
    
    /// <summary>
    /// 读取工作簿的部分区域
    /// </summary>
    public abstract class SheetReader<TEntity> : ISheetReader
    {
        public TEntity Entity;
        public List<TEntity> Entities;
        public Msg Msg;

        protected int iRow;
        protected int iCol;
        protected int endRow;
        protected ExcelRange range;

        public abstract int StartRow { get; }
        public abstract IEnumerable<IReadCell> ReadRow();
        public abstract TEntity NewEntity();
        public abstract void ReadFinished();

        public virtual string GetMsg() { return Msg.Builder.ToString(); }
        public virtual void ResetICol() { iCol = 0; }
        public virtual void ReadEnd() { }

        public virtual bool BeforeRead(ExcelRange range, ExcelCellAddress end)
        {
            this.range = range;
            endRow = end.Row;
            iRow = StartRow - 1;
            Msg = new MsgHtml();
            Entities = new List<TEntity>(endRow - iRow);
            return true;
        }

        public virtual bool NextRow()
        {
            if (++iRow > endRow) { return false; }

            Msg.SetRow(iRow);
            ResetICol();
            Entity = NewEntity();       // NewEntity() 可以放在迭代器里面, IEnumerable<IReadCell> ReadRow(), 因为迭代器可以跳出 ReadRow
            return true;
        }

        public virtual void ReadRange()
        {
            while (NextRow())
            {
                foreach (var cell in ReadRow())
                {
                    cell.Reading(Msg, range[iRow, ++iCol].Value);
                }
            }
            ReadEnd();
        }

    }

}
