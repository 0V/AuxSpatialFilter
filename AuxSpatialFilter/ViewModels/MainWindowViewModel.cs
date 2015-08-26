using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;

using AuxSpatialFilter.Models;
using System.Windows;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace AuxSpatialFilter.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        /* コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ(.NET4.5ではlpropn)
         *  
         * を使用してください。
         * 
         * Modelが十分にリッチであるならコマンドにこだわる必要はありません。
         * View側のコードビハインドを使用しないMVVMパターンの実装を行う場合でも、ViewModelにメソッドを定義し、
         * LivetCallMethodActionなどから直接メソッドを呼び出してください。
         * 
         * ViewModelのコマンドを呼び出せるLivetのすべてのビヘイビア・トリガー・アクションは
         * 同様に直接ViewModelのメソッドを呼び出し可能です。
         */

        /* ViewModelからViewを操作したい場合は、View側のコードビハインド無で処理を行いたい場合は
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信する事を検討してください。
         */

        /* Modelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedEventListenerや
         * CollectionChangedEventListenerを使うと便利です。各種ListenerはViewModelに定義されている
         * CompositeDisposableプロパティ(LivetCompositeDisposable型)に格納しておく事でイベント解放を容易に行えます。
         * 
         * ReactiveExtensionsなどを併用する場合は、ReactiveExtensionsのCompositeDisposableを
         * ViewModelのCompositeDisposableプロパティに格納しておくのを推奨します。
         * 
         * LivetのWindowテンプレートではViewのウィンドウが閉じる際にDataContextDisposeActionが動作するようになっており、
         * ViewModelのDisposeが呼ばれCompositeDisposableプロパティに格納されたすべてのIDisposable型のインスタンスが解放されます。
         * 
         * ViewModelを使いまわしたい時などは、ViewからDataContextDisposeActionを取り除くか、発動のタイミングをずらす事で対応可能です。
         */

        /* UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         * 
         * LivetのViewModelではプロパティ変更通知(RaisePropertyChanged)やDispatcherCollectionを使ったコレクション変更通知は
         * 自動的にUIDispatcher上での通知に変換されます。変更通知に際してUIDispatcherを操作する必要はありません。
         */

        public void Initialize()
        {
        }

        public MainWindowViewModel()
        {
            RadioChecks = new ObservableSynchronizedCollection<bool>() { true, false, false };

            FileUris = new ObservableSynchronizedCollection<string>();
            KernelSource = new double[7][] 
            {
                new double[7], new double[7], new double[7], new double[7], new double[7], new double[7], new double[7] 
            };

            KernelSource = new double[][]
            {
                new double[]{ 0,0,0,0,0,0,0},
                new double[]{ 0,0,0,0,0,0,0},
                new double[]{ 0,0,1,0,-1,0,0},
                new double[]{ 0,0,1,0,-1,0,0},
                new double[]{ 0,0,1,0,-1,0,0},
                new double[]{ 0,0,0,0,0,0,0},
                new double[]{ 0,0,0,0,0,0,0}
            };
        }

        #region RadioChecks変更通知プロパティ
        private ObservableSynchronizedCollection<bool> _RadioChecks;

        public ObservableSynchronizedCollection<bool> RadioChecks
        {
            get
            { return _RadioChecks; }
            set
            {
                if (_RadioChecks == value)
                    return;
                _RadioChecks = value;
                RaisePropertyChanged("RadioChecks");
            }
        }
        #endregion

        public int RadioCheckIndex()
        {
            return RadioChecks.IndexOf(true);
        }

        public int GetKernelSize()
        {
            return (RadioCheckIndex() + 1) * 2 + 1;
        }


        #region FileNames変更通知プロパティ
        private ObservableSynchronizedCollection<string> _FileNames;

        public ObservableSynchronizedCollection<string> FileUris
        {
            get
            { return _FileNames; }
            set
            {
                if (_FileNames == value)
                    return;
                _FileNames = value;
                RaisePropertyChanged("FileNames");
            }
        }
        #endregion


        #region Kernel変更通知プロパティ
        private double[][] _Kernel;

        public double[][] KernelSource
        {
            get
            { return _Kernel; }
            set
            {
                if (_Kernel == value)
                    return;
                _Kernel = value;
                RaisePropertyChanged("Kernel");
            }
        }
        #endregion

        #region ExcuteCommand
        private ViewModelCommand _ExcuteCommand;

        public ViewModelCommand ExcuteCommand
        {
            get
            {
                if (_ExcuteCommand == null)
                {
                    _ExcuteCommand = new ViewModelCommand(Excute, CanExcute);
                }
                return _ExcuteCommand;
            }
        }

        public bool CanExcute()
        {
            return true;
            //            return FileUris.Count > 0;
        }

        public void Excute()
        {
            var count = FileUris.Count;
            Task.Run(() =>
            {
                for (int i = 0; i < count && FileUris.Count > 0; i++)
                {
                    int size = GetKernelSize();

                    var kernel = new SpatialFilterKernel(SourceToKernel(), size);
                    var images = ImageManager.Filter(FileUris[0], kernel);
                    images.Freeze();
                    var viewmodel = new ImageWindowViewModel(images);
                    Messenger.Raise(new TransitionMessage(viewmodel, "ImageWindow"));
                    App.Current.Dispatcher.Invoke(() => Delete(i));
                }
            });
        }
        #endregion

        public double[] SourceToKernel()
        {
            int size = GetKernelSize();
            var kernel = new double[size * size];
            var index = RadioCheckIndex();
            int count = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[count] = KernelSource[i + (2 - index)][j + (2 - index)];
                    count++;
                }
            }
            return kernel;
        }

        public void OpenFile(OpeningFileSelectionMessage m)
        {
            if (m.Response == null)
            {
                Messenger.Raise(new InformationMessage("Cancel", "Error", MessageBoxImage.Error, "Info"));
                return;
            }

            m.Response.Select(f => { FileUris.Add(f); return f; }).ToList();
        }


        #region DeleteCommand
        private ListenerCommand<int> _DeleteCommand;

        public ListenerCommand<int> DeleteCommand
        {
            get
            {
                if (_DeleteCommand == null)
                {
                    _DeleteCommand = new ListenerCommand<int>(Delete, CanDelete);
                }
                return _DeleteCommand;
            }
        }

        public bool CanDelete()
        {
            return true;
            //            return FileUris.Count > 0;
        }

        public void Delete(int parameter)
        {
            if (FileUris.Count <= parameter) return;

            FileUris.RemoveAt(parameter);
        }
        #endregion

    }
}
