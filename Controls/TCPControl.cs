namespace Smacoteria.Controls;
using Newtonsoft.Json;
using Smacoteria.Model;
using Smacoteria.ViewModel;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

public class TCPControl
{
    private string DefaultAddress = "127.0.0.1";
    private int Port = 2000;
    private ViewModelMainWindow viewModelMainWindow;
    public List<Dish> receivedListDishes = new List<Dish>();
    public List<Addition> receivedListAdditions = new List<Addition>();
    public static TcpClient TcpClient = new TcpClient();
    public int tableNumber;
    private Thread _thread;
    private static bool _isRunning = true;

    public TCPControl(ViewModelMainWindow viewModelMainWindow)
    {
        this.viewModelMainWindow = viewModelMainWindow;
        _ = this.Connect();
        this.ReceiveListsFromServer();
        this._thread = new Thread(ChangesInDb);
        this._thread.Start(viewModelMainWindow);
    }

    public virtual void Disconnect()
    {
        _isRunning = false;
        this._thread?.Join();
        TcpClient.Close();
    }

    public void ReceiveListsFromServer()
    {
        try
        {
            byte[] tableNumberBytes = new byte[4];
            TcpClient.GetStream().Read(tableNumberBytes, 0, tableNumberBytes.Length);
            int tableNumber = BitConverter.ToInt32(tableNumberBytes, 0);

            byte[] sizeBytes = new byte[4];
            TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length);
            int dishesSize = BitConverter.ToInt32(sizeBytes, 0);

            byte[] dishBytes = new byte[dishesSize];
            TcpClient.GetStream().Read(dishBytes, 0, dishBytes.Length);
            string dishesJson = Encoding.UTF8.GetString(dishBytes);

            TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length);
            int additionsSize = BitConverter.ToInt32(sizeBytes, 0);

            byte[] additionBytes = new byte[additionsSize];
            TcpClient.GetStream().Read(additionBytes, 0, additionBytes.Length);
            string additionsJson = Encoding.UTF8.GetString(additionBytes);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            this.tableNumber = tableNumber;
            this.receivedListDishes = JsonConvert.DeserializeObject<List<Dish>>(dishesJson);
            this.receivedListAdditions = JsonConvert.DeserializeObject<List<Addition>>(additionsJson);
        }
        catch (Exception ex)
        {
        }
    }

    public static async void ChangesInDb(object obj)
    {
        ViewModelMainWindow viewModelMainWindow = (ViewModelMainWindow)obj;
        byte[] bytesBuffer = new byte[4];
        int i;
        while (_isRunning)
        {
            try
            {
                i = await TcpClient.GetStream().ReadAsync(bytesBuffer, 0, bytesBuffer.Length);
                if (i == 0)
                {
                    break;
                }

                int dataType = BitConverter.ToInt32(bytesBuffer, 0);
                if (dataType == 0)
                {
                    HandleDish(viewModelMainWindow);
                }
                else if (dataType == 1)
                {
                    HandleAddition(viewModelMainWindow);
                }

                Array.Clear(bytesBuffer, 0, bytesBuffer.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                break;
            }
        }
    }

    public static void HandleDish(ViewModelMainWindow viewModelMainWindow)
    {
        try
        {
            byte[] typeBuffer = new byte[4];
            TcpClient.GetStream().Read(typeBuffer, 0, typeBuffer.Length);
            int dishType = BitConverter.ToInt32(typeBuffer, 0);
            switch (dishType)
            {
                case 0:
                    AddDish(viewModelMainWindow);
                    break;
                case 1:
                    UpdateDish(viewModelMainWindow);
                    break;
                case 2:
                    DeleteDish(viewModelMainWindow);
                    break;
                default: break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static void HandleAddition(ViewModelMainWindow viewModelMainWindow)
    {
        try
        {
            byte[] typeBuffer = new byte[4];
            TcpClient.GetStream().Read(typeBuffer, 0, typeBuffer.Length);
            int additionType = BitConverter.ToInt32(typeBuffer, 0);
            switch (additionType)
            {
                case 0:
                    AddAddition(viewModelMainWindow);
                    break;
                case 1:
                    UpdateAddition(viewModelMainWindow);
                    break;
                case 2:
                    DeleteAddition(viewModelMainWindow);
                    break;
                default: break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    public static void AddDish(ViewModelMainWindow viewModelMainWindow)
    {
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] dishBytes = new byte[dataSize];
        TcpClient.GetStream().Read(dishBytes, 0, dishBytes.Length);
        string dishJson = Encoding.UTF8.GetString(dishBytes);
        Dish newDish = JsonConvert.DeserializeObject<Dish>(dishJson);

        if (newDish.CategoryId == 1)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModelMainWindow._croissantSandwiches.Add(newDish);
            });
        }
        else if (newDish.CategoryId == 2)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModelMainWindow._sweetCroissants.Add(newDish);
            });
        }
        else if (newDish.CategoryId == 3)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModelMainWindow._drinks.Add(newDish);
            });
        }

        if (newDish.Photo != null)
        {
            byte[] sizeBytes = new byte[4];
            int i;
            int fileSize;
            if ((i = TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length)) != 0)
            {
                try
                {
                    fileSize = BitConverter.ToInt32(sizeBytes, 0);
                    ReceivePhoto(fileSize, newDish.Photo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving photo: {ex.Message}");
                }
            }
        }
    }

    public static void UpdateDish(ViewModelMainWindow viewModelMainWindow)
    {
        bool isPhotoSame = true;
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] dishBytes = new byte[dataSize];
        TcpClient.GetStream().Read(dishBytes, 0, dishBytes.Length);
        string dishJson = Encoding.UTF8.GetString(dishBytes);
        Dish newDish = JsonConvert.DeserializeObject<Dish>(dishJson);

        Dish foundDish = FindDishInLists(viewModelMainWindow, newDish.Id);
        if (foundDish != null)
        {
            if (foundDish.Photo != newDish.Photo)
                isPhotoSame = false;
            foundDish.Name = newDish.Name;
            foundDish.Description = newDish.Description;
            foundDish.Price = newDish.Price;
            foundDish.Count = newDish.Count;
            foundDish.CountType = newDish.CountType;
            foundDish.Category = newDish.Category;
            foundDish.Photo = newDish.Photo;
        }

        if (isPhotoSame == false && foundDish.Photo != null)
        {
            byte[] sizeBytes = new byte[4];
            int i;
            int fileSize;
            if ((i = TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length)) != 0)
            {
                try
                {
                    fileSize = BitConverter.ToInt32(sizeBytes, 0);
                    ReceivePhoto(fileSize, newDish.Photo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving photo: {ex.Message}");
                }
            }
        }
    }


    public static void DeleteDish(ViewModelMainWindow viewModelMainWindow)
    {
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] dishBytes = new byte[dataSize];
        TcpClient.GetStream().Read(dishBytes, 0, dishBytes.Length);
        string dishJson = Encoding.UTF8.GetString(dishBytes);
        Dish dish = JsonConvert.DeserializeObject<Dish>(dishJson);
        RemoveDishFromList(viewModelMainWindow, dish.Id);

        if (!string.IsNullOrEmpty(dish.Photo))
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(currentDirectory)));

                string filePath = (projectPath + dish.Photo).Replace('/', '\\');
                string source = dish.Photo.Substring(1).Replace('/', '\\');
                File.Delete(dish.Photo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting photo: {ex.Message}");
            }
        }
    }


    public static void AddAddition(ViewModelMainWindow viewModelMainWindow)
    {
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] additionBytes = new byte[dataSize];
        TcpClient.GetStream().Read(additionBytes, 0, additionBytes.Length);
        string additionJson = Encoding.UTF8.GetString(additionBytes);
        Addition newAddition = JsonConvert.DeserializeObject<Addition>(additionJson);

        Application.Current.Dispatcher.Invoke(() =>
        {
            viewModelMainWindow._additions.Add(newAddition);
        });

        if (newAddition.Photo != null)
        {
            byte[] sizeBytes = new byte[4];
            int i;
            int fileSize;
            if ((i = TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length)) != 0)
            {
                try
                {
                    fileSize = BitConverter.ToInt32(sizeBytes, 0);
                    ReceivePhoto(fileSize, newAddition.Photo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving photo: {ex.Message}");
                }
            }
        }
    }

    public static void UpdateAddition(ViewModelMainWindow viewModelMainWindow)
    {
        bool isPhotoSame = true;
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] additionBytes = new byte[dataSize];
        TcpClient.GetStream().Read(additionBytes, 0, additionBytes.Length);
        string additionJson = Encoding.UTF8.GetString(additionBytes);
        Addition newAddition = JsonConvert.DeserializeObject<Addition>(additionJson);
        var foundAddition = viewModelMainWindow._additions.FirstOrDefault(d => d.Id == newAddition.Id);
        if (foundAddition != null)
        {
            if (foundAddition.Photo != newAddition.Photo)
                isPhotoSame = false;

            foundAddition.Name = newAddition.Name;
            foundAddition.Price = newAddition.Price;
            foundAddition.Count = newAddition.Count;
            foundAddition.CountType = newAddition.CountType;
            foundAddition.CategoryId = newAddition.CategoryId;
            foundAddition.Photo = newAddition.Photo;
        }

        if (isPhotoSame == false && foundAddition.Photo != null)
        {
            byte[] sizeBytes = new byte[4];
            int i;
            int fileSize;
            if ((i = TcpClient.GetStream().Read(sizeBytes, 0, sizeBytes.Length)) != 0)
            {
                try
                {
                    fileSize = BitConverter.ToInt32(sizeBytes, 0);
                    ReceivePhoto(fileSize, newAddition.Photo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving photo: {ex.Message}");
                }
            }
        }
    }

    public static void DeleteAddition(ViewModelMainWindow viewModelMainWindow)
    {
        byte[] sizeBuffer = new byte[4];
        TcpClient.GetStream().Read(sizeBuffer, 0, sizeBuffer.Length);
        int dataSize = BitConverter.ToInt32(sizeBuffer, 0);

        byte[] additionBytes = new byte[dataSize];
        TcpClient.GetStream().Read(additionBytes, 0, additionBytes.Length);
        string additionJson = Encoding.UTF8.GetString(additionBytes);
        Addition addition = JsonConvert.DeserializeObject<Addition>(additionJson);
        var foundAddition = viewModelMainWindow._additions.FirstOrDefault(d => d.Id == addition.Id);
        if (foundAddition != null)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModelMainWindow._additions.Remove(foundAddition);
            });
        }
    }

    public void SendOrderToServer(Order order)
    {
        if (!TcpClient.Connected)
        {
            return;
        }

        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            string jsonString = JsonConvert.SerializeObject(order, settings);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            byte[] sizeBytes = BitConverter.GetBytes(jsonBytes.Length);

            TcpClient.GetStream().Write(sizeBytes, 0, sizeBytes.Length);
            TcpClient.GetStream().Write(jsonBytes, 0, jsonBytes.Length);

        }
        catch (Exception ex)
        {
        }
    }

    public async Task Connect()
    {
        IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Parse(this.DefaultAddress), this.Port);
        await TcpClient.ConnectAsync(remoteIpEndPoint.Address, remoteIpEndPoint.Port);
    }

    public static void ReceivePhoto(int fileSize, string path)
    {
        byte[] photoBytes = new byte[fileSize];
        TcpClient.GetStream().Read(photoBytes, 0, fileSize);
        string currentDirectory = Directory.GetCurrentDirectory();
        string projectPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(currentDirectory)));

        string filePath = (projectPath + path).Replace('/', '\\');
        string source = path.Substring(1).Replace('/', '\\');
        RsourcesControl.AddResourceToProjectFile(projectPath, source);
        File.WriteAllBytes(filePath, photoBytes);
    }

    private static void RemoveDishFromList(ViewModelMainWindow viewModelMainWindow, int dishId)
    {
        ObservableCollection<Dish>[] allLists = new ObservableCollection<Dish>[]
        {
            viewModelMainWindow._croissantSandwiches,
            viewModelMainWindow._sweetCroissants,
            viewModelMainWindow._drinks,
        };

        Application.Current.Dispatcher.Invoke(() =>
        {
            foreach (var list in allLists)
            {
                var foundDish = list.FirstOrDefault(d => d.Id == dishId);
                if (foundDish != null)
                {
                    list.Remove(foundDish);
                    return;
                }
            }
        });
    }

    public static Dish FindDishInLists(ViewModelMainWindow viewModelMainWindow, int dishId)
    {
        ObservableCollection<Dish>[] allLists = new ObservableCollection<Dish>[]
        {
            viewModelMainWindow._croissantSandwiches,
            viewModelMainWindow._sweetCroissants,
            viewModelMainWindow._drinks,
        };
        foreach (var list in allLists)
        {
            var foundDish = list.FirstOrDefault(d => d.Id == dishId);
            if (foundDish != null)
            {
                return foundDish;
            }
        }

        return null;
    }
}