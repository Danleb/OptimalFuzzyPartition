using OptimalFuzzyPartition.Annotations;
using OptimalFuzzyPartitionAlgorithm.Settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OptimalFuzzyPartition.View
{
    public class CenterDataView : INotifyPropertyChanged
    {
        private readonly int _index;
        private CenterData _centerData;
        public int CenterNumber => _index + 1;

        public string ValueX
        {
            get =>
                IsFixedCenter ?
                    CenterData.Position[0].ToString() :
                    "---";
            set
            {
                if (double.TryParse(value, out var val))
                {
                    CenterData.Position[0] = val;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueY
        {
            get => IsFixedCenter ?
                CenterData.Position[0].ToString() :
                "---";
            set
            {
                if (double.TryParse(value, out var val))
                {
                    CenterData.Position[1] = val;
                    OnPropertyChanged();
                }
            }
        }

        public CenterData CenterData
        {
            get
            {
                if(_centerData == null)
                    _centerData = new CenterData();
                return _centerData;
            }
            set => _centerData = value;
        }

        public bool IsPositionCellReadonly => !CenterData.IsFixed;

        public bool IsFixedCenter
        {
            get => CenterData.IsFixed;
            set
            {
                CenterData.IsFixed = value;
                OnPropertyChanged(nameof(ValueX));
                OnPropertyChanged(nameof(ValueY));
            }
        }

        public double CoefficientA
        {
            get => CenterData.A;
            set
            {
                CenterData.A = value;
                OnPropertyChanged();
            }
        }

        public double CoefficientW
        {
            get => CenterData.W;
            set
            {
                CenterData.W = value;
                OnPropertyChanged();
            }
        }

        public CenterDataView(CenterData centerData, int index)
        {
            _index = index;
            CenterData = centerData;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}