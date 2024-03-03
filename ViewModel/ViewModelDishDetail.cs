namespace Smacoteria.ViewModel;
using GalaSoft.MvvmLight.Command;
using Smacoteria.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

public class ViewModelDishDetail : INotifyPropertyChanged
{
    public DishInOrder _dishInOrder = new DishInOrder();
    public float _totalCoast;

    private ViewModelMainWindow viewModelMainWindow;
    private List<Addition> _addedadditions;
    private ObservableCollection<Addition> _alladditions;
    private ViewModelMainWindow _mainWindowViewModel;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand AddToBasket { get; set; }

    public List<Addition> AddedAdditions
    {
        get
        {
            return this._addedadditions;
        }

        set
        {
            this._addedadditions = value;
            this.OnPropertyChanged(nameof(this.AddedAdditions));
        }
    }

    public DishInOrder DishInOrder
    {
        get
        {
            return this._dishInOrder;
        }

        set
        {
            this._dishInOrder = value;
            this.OnPropertyChanged(nameof(this.DishInOrder));
        }
    }

    public float TotalCoast
    {
        get
        {
            return this._totalCoast;
        }

        set
        {
            this._totalCoast = value;
            this.OnPropertyChanged(nameof(this.TotalCoast));
        }
    }

    public List<Addition> Additions
    {
        get
        {
            return this._dishInOrder.Additions;
        }

        set
        {
            this._dishInOrder.Additions = value;
            this.OnPropertyChanged(nameof(this.Additions));
        }
    }

    public ObservableCollection<Addition> AllAdditions
    {
        get
        {
            return this._alladditions;
        }

        set
        {
            this._alladditions = value;
            this.OnPropertyChanged(nameof(this.AllAdditions));
        }
    }

    public ViewModelDishDetail()
    {

    }

    public ViewModelDishDetail(object selectedDish, ObservableCollection<Addition> additions, ViewModelMainWindow mainWindowViewModel)
    {
        this.DishInOrder.Dish = (Dish)selectedDish;
        this.DishInOrder.TotalCoast = this.DishInOrder.Dish.Price;
        this._alladditions = additions;
        this.TotalCoast = this.DishInOrder.TotalCoast;
        this.AddToBasket = new RelayCommand(this.ExecuteAddToBasket, this.CanExecuteAddToBasket);
        this.viewModelMainWindow = mainWindowViewModel;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void ExecuteAdditionClick(Addition selectedAddition)
    {
        this.DishInOrder.Additions.Add(selectedAddition);
        this.AllAdditions.Remove(selectedAddition);
        this.DishInOrder.TotalCoast += selectedAddition.Price;
        this.TotalCoast = this.DishInOrder.TotalCoast;
        this.AddedAdditions = this.DishInOrder.Additions.ToList();
    }

    public void ExecuteAdditionDelete(Addition selectedAddition)
    {
        this.DishInOrder.Additions.Remove(selectedAddition);
        this.DishInOrder.TotalCoast -= selectedAddition.Price;
        this.TotalCoast = this.DishInOrder.TotalCoast;
        this.AddedAdditions = this.DishInOrder.Additions.ToList();
        this.AllAdditions.Insert(0, selectedAddition);
    }

    private bool CanExecuteAddToBasket()
    {
        return true;
    }

    private void ExecuteAddToBasket()
    {
        this.viewModelMainWindow.order.Dishes.Add(this.DishInOrder);
        this.viewModelMainWindow.DishesInOrder.Add(this.DishInOrder);
        this.viewModelMainWindow.order.TotalCost += this.DishInOrder.TotalCoast;
        this.viewModelMainWindow.TotalCoast = this.viewModelMainWindow.order.TotalCost;

    }
}