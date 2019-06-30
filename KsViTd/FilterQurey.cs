using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KsViTd {
    class Filter<T> {
        public int[] comIds;
    }
    class StatisticalBase {
        public int CluCount;
        public int Delivery;
    }
    class StatisticalCm {
        public string CarModel;

    }


    interface IReport<T> {
        IList<T> SqlQurey(Filter<T> filter);
    }
    class DReport :IReport<StatisticalBase>, IReport<StatisticalCm> {
        

        //public IList<T> SqlQurey<T>(Filter<T> filter) {
        //    return SqlQurey<T>(filter);
        //}

        public IList<StatisticalBase> SqlQurey(Filter<StatisticalBase> filter) {
            return new[] { new StatisticalBase { CluCount = 10, Delivery = 5 } };
        }
        public IList<StatisticalCm> SqlQurey(Filter<StatisticalCm> filter) {
            return new[] { new StatisticalCm { CarModel = "308Tx" } };
        }

        //IList<StatisticalBase> IReport<StatisticalBase>.SqlQurey(Filter<StatisticalBase> filter) {
        //    throw new NotImplementedException();
        //}
    }
}
