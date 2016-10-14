    private void sp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == this.tableGridContent)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(this.tableGridContent);
            }
 
        }
        private void sp_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                e.Effects = DragDropEffects.Move;
            }
        }
        private void sp_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) && 
                    ((Math.Abs(e.GetPosition(this.tableGridContent).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(this.tableGridContent).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                {
                    _isDragging = true;
                    _realDragSource = e.Source as UIElement;
                    _realDragSource.CaptureMouse();
                    DragDrop.DoDragDrop(_realDragSource, new DataObject("UIElement", e.Source, true), DragDropEffects.Move);
                }
            }
        }
        private void sp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDown = false;
            _isDragging = false;
            _realDragSource.ReleaseMouseCapture();
        }
        private void sp_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("UIElement"))
            {
                UIElement droptarget = e.Source as UIElement;
                int droptargetIndex = -1, i = 0;
                foreach (UIElement element in this.tableGridContent.Children)
                {
                    if (element.Equals(droptarget))
                    {
                        droptargetIndex = i;
                        break;
                    }
                    i++;
                }
                if (droptargetIndex != -1)
                {
                    var originIndex = MainWindow.CharList.IndexOf(MainWindow.CharList.Find(j => j.Name.Equals((_realDragSource as newStrip).Tag)));
                    Character temp = new Character();
                    temp = MainWindow.CharList[originIndex];

                    this.tableGridContent.Children.Remove(_realDragSource);
                    this.tableGridContent.Children.Insert(droptargetIndex, _realDragSource);
                    MainWindow.CharList.RemoveAt(originIndex);
                    MainWindow.CharList.Insert(droptargetIndex, temp);

                }

                _isDown = false;
                _isDragging = false;
                _realDragSource.ReleaseMouseCapture();
                customCursor = null;
                _realDragSource.Visibility = Visibility.Visible;
            }
        }

        private Cursor customCursor = null;
        private void Give_Feedback(object sender, GiveFeedbackEventArgs e)
        {

            if (e.Effects == DragDropEffects.Move)
            {
                if (customCursor == null)

                    customCursor = CursorHelper.CreateCursor(e.Source as UIElement);

                if (customCursor != null)

                    e.UseDefaultCursors = false;
                Mouse.SetCursor(customCursor);
            }
            else
                e.UseDefaultCursors = true;

                e.Handled = true;
                _realDragSource.Visibility = Visibility.Hidden;
            }
           
        


        public class CursorHelper
        {
            private static class NativeMethods
            {
                public struct IconInfo
                {
                    public bool fIcon;
                    public int xHotspot;
                    public int yHotspot;
                    public IntPtr hbmMask;
                    public IntPtr hbmColor;
                }

                [DllImport("user32.dll")]
                public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

                [DllImport("user32.dll")]
                public static extern bool DestroyIcon(IntPtr hIcon);

                [DllImport("user32.dll")]
                [return: MarshalAs(UnmanagedType.Bool)]
                public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
            }

            [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
            private class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
            {
                public SafeIconHandle()
                    : base(true)
                {
                }

                override protected bool ReleaseHandle()
                {
                    return NativeMethods.DestroyIcon(handle);
                }
            }

            private static Cursor InternalCreateCursor(System.Drawing.Bitmap bmp)
            {
                var iconInfo = new NativeMethods.IconInfo();
                NativeMethods.GetIconInfo(bmp.GetHicon(), ref iconInfo);

                iconInfo.xHotspot = 700;
                iconInfo.yHotspot = 15;
                iconInfo.fIcon = false;

                SafeIconHandle cursorHandle = NativeMethods.CreateIconIndirect(ref iconInfo);
                return CursorInteropHelper.Create(cursorHandle);
            }

            public static Cursor CreateCursor(UIElement element)
            {
                element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                element.Arrange(new Rect(new Point(), element.DesiredSize));

                RenderTargetBitmap rtb =
                  new RenderTargetBitmap(
                    (int)element.DesiredSize.Width,
                    (int)element.DesiredSize.Height,
                    96, 96, PixelFormats.Pbgra32);

                rtb.Render(element);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using (var ms = new System.IO.MemoryStream())
                {
                    encoder.Save(ms);
                    using (var bmp = new System.Drawing.Bitmap(ms))
                    {
                        return InternalCreateCursor(bmp);
                    }
                }
            }
}



            <Grid Name = "TabHeader" Margin="0" Grid.Row="2" Background="#FF506d7b">
                <Grid.CacheMode>
                    <BitmapCache/>
                </Grid.CacheMode>
                <Grid.Effect>
                    <DropShadowEffect ShadowDepth = "2.5" Opacity="0.2" BlurRadius="6" Direction="270"/>
                </Grid.Effect>
                <!--#region Rows/Columns Defs-->
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width = "*" />
                    < ColumnDefinition Width="110"/>
                    <ColumnDefinition Width = "60" />
                    < ColumnDefinition Width="110"/>
                    <ColumnDefinition Width = "*" />
                </ Grid.ColumnDefinitions >
                < Grid.RowDefinitions >
                    < RowDefinition Height="*"/>
                    <RowDefinition Height = "3" />
                </ Grid.RowDefinitions >
                < !--
#endregion-->
                < TextBlock Grid.Column="1" Text="OVERVIEW"  Foreground="White" FontSize="15" VerticalAlignment="Center" TextAlignment="Center" MouseDown="tab_Clicked" Cursor="Hand" FontWeight="DemiBold" Height="20" Margin="0,4,0,3"/>
                <TextBlock Grid.Column="2" Text= "EDIT"      Foreground= "White" FontSize= "15" VerticalAlignment= "Center" TextAlignment= "Center" MouseDown= "tab_Clicked" Cursor= "Hand" FontWeight= "DemiBold" Height= "20" Margin= "0,4,0,3" />
                < TextBlock Grid.Column= "3" Text= "DUNGEONS"  Foreground= "White" FontSize= "15" VerticalAlignment= "Center" TextAlignment= "Center" MouseDown= "tab_Clicked" Cursor= "Hand" FontWeight= "DemiBold" Height= "20" Margin= "0,4,0,3" />
                < Grid Grid.Row= "1" Grid.Column= "1" Grid.ColumnSpan= "3" >
                    < Rectangle Name= "selectedTab"  Width= "110" Fill= "#FFFF782A" HorizontalAlignment= "Left" VerticalAlignment= "Stretch" />
                </ Grid >
            </ Grid >
