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
using System.Drawing;
using System.Windows.Media.Imaging;

namespace AuxSpatialFilter.ViewModels
{
    public class ImageWindowViewModel : ViewModel
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





        #region Images変更通知プロパティ
        private ImageData _Images;

        public ImageData Images
        {
            get
            { return _Images; }
            set
            {
                if (_Images == value)
                    return;
                _Images = value;
                RaisePropertyChanged("Images");
            }
        }
        #endregion

        public ImageWindowViewModel()
        {
            SourceFileName = "source.png";
            ResultFileName = "result.png";
        }

        public ImageWindowViewModel(ImageData data)
        {
            Images = data;
            SourceFileName = "source.png";
            ResultFileName = "result.png";
        }

        public void Initialize()
        {
        }

        #region SourceImage変更通知プロパティ

        public WriteableBitmap SourceImage
        {
            get
            { return Images.SourceImage; }
        }
        #endregion


        #region ResultImagae変更通知プロパティ
        public WriteableBitmap ResultImagae
        {
            get
            { return Images.ResultImage; }
        }
        #endregion



        #region SourceFileName変更通知プロパティ
        private string _SourceFileName;

        public string SourceFileName
        {
            get
            { return _SourceFileName; }
            set
            {
                if (_SourceFileName == value)
                    return;
                _SourceFileName = value;
                RaisePropertyChanged("SourceFileName");
            }
        }
        #endregion


        #region ResultFileName変更通知プロパティ
        private string _ResultFileName;

        public string ResultFileName
        {
            get
            { return _ResultFileName; }
            set
            {
                if (_ResultFileName == value)
                    return;
                _ResultFileName = value;
                RaisePropertyChanged("ResultFileName");
            }
        }
        #endregion


        #region SaveSourceImageCommand
        private ViewModelCommand _SaveSourceImageCommand;

        public ViewModelCommand SaveSourceImageCommand
        {
            get
            {
                if (_SaveSourceImageCommand == null)
                {
                    _SaveSourceImageCommand = new ViewModelCommand(SaveSourceImage, CanSaveSourceImage);
                }
                return _SaveSourceImageCommand;
            }
        }

        public bool CanSaveSourceImage()
        {
            return SourceFileName != null && !string.IsNullOrWhiteSpace(SourceFileName);
        }

        public void SaveSourceImage()
        {
            Images.SaveSource(SourceFileName);
        }
        #endregion




        #region SaveResultImageCommand
        private ViewModelCommand _SaveResultImageCommand;

        public ViewModelCommand SaveResultImageCommand
        {
            get
            {
                if (_SaveResultImageCommand == null)
                {
                    _SaveResultImageCommand = new ViewModelCommand(SaveResultImage, CanSaveResultImage);
                }
                return _SaveResultImageCommand;
            }
        }

        public bool CanSaveResultImage()
        {
            return ResultFileName != null && !string.IsNullOrWhiteSpace(ResultFileName);

        }

        public void SaveResultImage()
        {
            Images.SaveResult(ResultFileName);
        }
        #endregion


    }
}
