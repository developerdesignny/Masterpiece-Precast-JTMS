using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using JTMS.Helpers;
using JTMS.Models;
using JTMS.ViewModels.DialogsVM;
using Syncfusion.Pdf.Barcode;
using Syncfusion.Pdf;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using JTMS.Dialogs;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf.Graphics;
using OfficeOpenXml;
using Microsoft.Win32;
using System.Windows.Media.Media3D;

namespace JTMS.ViewModels
{
    class MoldViewModel : ObservableObject
    {
        private ProjectModel _currentProject;
        public ProjectModel CurrentProject { get => _currentProject; set => SetProperty(ref _currentProject, value); }

        private ObservableCollection<MoldModel> _molds = new ObservableCollection<MoldModel>();
        public ObservableCollection<MoldModel> Molds { get => _molds; set => SetProperty(ref _molds, value); }

        private ObservableCollection<ProjectNotesModels> _projectNotes = new ObservableCollection<ProjectNotesModels>();
        public ObservableCollection<ProjectNotesModels> ProjectNotes { get => _projectNotes; set => SetProperty(ref _projectNotes, value); }

        private double _percentageCleared = 0.0;
        public double PercentageCleared { get => _percentageCleared; set => SetProperty(ref _percentageCleared, value); }

        public RelayCommand AddMoldCommand { get; set; }
        public RelayCommand ReloadCommand { get; set; }
        public RelayCommand GoBackCommand { get; set; }
        public IRelayCommand PrintBarcodeCommand { get; set; }
        public IRelayCommand EditMoldCommand { get; set; }
        public IRelayCommand DeleteMoldCommand { get; set; }
        public IRelayCommand OpenMoldCommand { get; set; }
        public RelayCommand AddNewNoteCommand { get; set; }
        public RelayCommand ViewPalletsCommand { get; set; }
        public RelayCommand PrintPalletFilledCMD { get; set; }
        public RelayCommand PrintReportCMD { get; set; }
        public RelayCommand PrintAllCommand { get; set; }
        public RelayCommand PrintProgressReportCMD { get; set; }
        public RelayCommand ImportMoldsCommand { get; set; }
        public IRelayCommand EditNoteCommand { get; set; }
        public IRelayCommand DeleteNoteCommand { get; set; }

        private DataHandler.DataHandler dataHandler;
        private Dictionary<string, int> MoldHeaders = new Dictionary<string, int>();

        public MoldViewModel()
        {
            dataHandler = new DataHandler.DataHandler();
            GoBackCommand = new RelayCommand(goBack);
            AddMoldCommand = new RelayCommand(addMold);
            ReloadCommand = new RelayCommand(LoadMoldData);
            ViewPalletsCommand = new RelayCommand(viewPallets);
            PrintPalletFilledCMD = new RelayCommand(filledPalletCodes);
            PrintBarcodeCommand = new RelayCommand<MoldModel>(model => printBarcode(model));
            EditMoldCommand = new RelayCommand<MoldModel>(model => editMoldData(model));
            DeleteMoldCommand = new RelayCommand<MoldModel>(model => delteMoldData(model));
            OpenMoldCommand = new RelayCommand<MoldModel>(model => openDetails(model));
            ImportMoldsCommand = new RelayCommand(importMolds);
            PrintReportCMD = new RelayCommand(printReport);
            PrintAllCommand = new RelayCommand(printallCMD);
            PrintProgressReportCMD = new RelayCommand(showProgressReport);
            AddNewNoteCommand = new RelayCommand(addNewNote);
            EditNoteCommand = new RelayCommand<ProjectNotesModels>(obj => editNote(obj));
            DeleteNoteCommand = new RelayCommand<ProjectNotesModels>(obj => deleteNote(obj));
            WeakReferenceMessenger.Default.Register<DataMessageModel>(this, (r, model) => recieveViewData(model));
        }

        private void deleteNote(ProjectNotesModels noteModel)
        {
            ProjectNotes.Remove(noteModel);
            dataHandler.context.ProjectNotes.Remove(noteModel);
            dataHandler.context.SaveChanges();
        }

        private void editNote(ProjectNotesModels noteModel)
        {
            var win = new ProjectNotesWin();
            win.projDescription.Text = noteModel.Description;
            win.projName.Text = noteModel.Name;
            win.ProjectNote.Text = noteModel.Note;
            win.ShowDialog();
            if (win.Issaved)
            {
                var model = dataHandler.context.ProjectNotes.FirstOrDefault(obj => obj.Id == noteModel.Id);
                model.Name = win.projName.Text;
                model.Description = win.projDescription.Text;
                model.Note = win.ProjectNote.Text;
                dataHandler.context.SaveChanges();
            }
        }

        private void addNewNote()
        {
            var win = new ProjectNotesWin();
            win.ShowDialog();
            if (win.Issaved)
            {
                var note = new ProjectNotesModels
                {
                    Name = win.projName.Text,
                    Description = win.projDescription.Text,
                    Note = win.ProjectNote.Text,
                    ProjectId = CurrentProject.Id,
                };
                dataHandler.context.ProjectNotes.Add(note);
                dataHandler.context.SaveChanges();
                ProjectNotes.Add(note);
            }
        }

        private async void printallCMD()
        {
            PdfDocument doc = new PdfDocument();
            PdfFont textFont = new PdfTrueTypeFont(new Font("Arial", 13, System.Drawing.FontStyle.Regular));
            var page = doc.Pages.Add();
            page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);

            var col = 5.0f;
            var row = 5.0f;

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float height = convertor.ConvertUnits(1.69f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            float width = convertor.ConvertUnits(6.28f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;

            foreach (var mold in Molds)
            {
                var subMolds = await dataHandler.GetSubMolds(mold);
                foreach (var sMold in subMolds)
                {
                    var barcode = new PdfCode128BBarcode();
                    barcode.Size = new SizeF(100, 40);
                    barcode.Text = $"{CurrentProject.JobName}:{mold.MoldCode}-{sMold.QIndex}";
                    barcode.Font = textFont;
                    barcode.TextDisplayLocation = TextLocation.None;
                    barcode.Draw(page, new PointF(col, row));
                    var val = $"{CurrentProject.JobName}:{mold.MoldCode}";
                    var textElement = new PdfTextElement(val, textFont, PdfBrushes.Black);
                    textElement.Draw(page, new RectangleF(col, row + 50, width, height));

                    col += width;
                    if (col >= (page.GetClientSize().Width - 50))
                    {
                        col = 5.0f;
                        row += (height * 2);

                        if (row >= (page.GetClientSize().Height - 50.0f))
                        {
                            row = 5.0f;
                            page = doc.Pages.Add();
                            page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
                        }
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            doc.Close(true);

            PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
            var printDocumentWin = new PrintDocumentWin();
            printDocumentWin.pdfViewer.Load(pdfLoadedDocument);
            printDocumentWin.ShowDialog();
        }

        private async void printBarcode(MoldModel model)
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.Pages.Add();
            page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
            PdfFont textFont = new PdfTrueTypeFont(new Font("Arial", 13, System.Drawing.FontStyle.Regular));

            var col = 5.0f;
            var row = 5.0f;

            var subMolds = await dataHandler.GetSubMolds(model);
            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float height = convertor.ConvertUnits(1.69f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            float width = convertor.ConvertUnits(6.28f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;

            foreach (var mold in subMolds)
            {

                var barcode = new PdfCode128BBarcode();
                barcode.Size = new SizeF(width - 10, height);
                barcode.Text = $"{CurrentProject.JobName}:{model.MoldCode}-{mold.QIndex}";
                barcode.Font = textFont;
                barcode.TextDisplayLocation = TextLocation.None;
                barcode.Draw(page, new PointF(col, row));
                var val = $"{CurrentProject.JobName}:{model.MoldCode}";
                var textElement = new PdfTextElement(val, textFont, null, PdfBrushes.Black, format);
                textElement.Draw(page, new RectangleF(col, row + 50, width, height));

                col += width;
                if (col >= (page.GetClientSize().Width - 50))
                {
                    col = 5.0f;
                    row += (height * 2);

                    if (row >= (page.GetClientSize().Height - 50.0f))
                    {
                        row = 5.0f;
                        page = doc.Pages.Add();
                        page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            doc.Close(true);

            PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
            var printDocumentWin = new PrintDocumentWin();
            printDocumentWin.pdfViewer.Load(pdfLoadedDocument);
            printDocumentWin.ShowDialog();
        }


        private async void openDetails(MoldModel mold)
        {
            var vm = new ViewMoldViewModel();
            vm.CurrentMold = mold;
            vm.initMoldList();
            var win = new ViewMolds(vm);
            win.ShowDialog();
        }

        private async void importMolds()
        {
            MoldHeaders = new Dictionary<string, int>()
            {
                {"Mold Amount",-1 },
                {"Mold Size",-1 },
                {"Mold Codes",-1 },
                {"Mold Cut",-1 },
                {"Mold Info",-1 },
                {"Cut",-1 },
                {"Paint",-1 },
            };

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Import Data";
                openFileDialog.CheckFileExists = false;
                openFileDialog.DefaultExt = "xlsx";
                openFileDialog.CheckPathExists = true;
                openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";

                if (openFileDialog.ShowDialog() == true)
                {
                    var path = openFileDialog.FileName;

                    using (var excelPack = new ExcelPackage(path))
                    {
                        ExcelWorksheet worksheet = excelPack.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet != null)
                        {
                            int colCount = worksheet.Dimension.End.Column;  //get Column Count
                            int rowCount = worksheet.Dimension.End.Row;     //get row count

                            for (int s = 0; s < MoldHeaders.Count; ++s)
                            {
                                var val = MoldHeaders.ElementAt(s);
                                for (int i = 1; i < colCount + 1; ++i)
                                {
                                    if (worksheet?.Cells[12, i]?.Value?.ToString()?.Trim() == val.Key.Trim())
                                    {
                                        MoldHeaders[val.Key] = i;
                                        break;
                                    }
                                }
                            }//

                            var molds = new List<MoldModel>();
                            for (int row = 13; row < rowCount + 1; ++row)
                            {
                                var mold = new MoldModel();

                                var amount = 0;
                                int.TryParse(worksheet?.Cells[row, MoldHeaders["Mold Amount"]].Value?.ToString()?.Trim(), out amount);
                                mold.MoldAmount = amount;
                                mold.MoldCode = (MoldHeaders["Mold Codes"] != -1) ? worksheet?.Cells[row, MoldHeaders["Mold Codes"]].Value?.ToString()?.Trim() : string.Empty;
                                mold.MoldInfo = (MoldHeaders["Mold Info"] != -1) ? worksheet?.Cells[row, MoldHeaders["Mold Info"]].Value?.ToString()?.Trim() : string.Empty;
                                mold.MoldSize = (MoldHeaders["Mold Size"] != -1) ? worksheet?.Cells[row, MoldHeaders["Mold Size"]].Value?.ToString()?.Trim() : string.Empty;
                                mold.Cut = (MoldHeaders["Cut"] != -1) ? worksheet?.Cells[row, MoldHeaders["Cut"]].Value?.ToString()?.Trim() : string.Empty;
                                mold.Paint = (MoldHeaders["Paint"] != -1) ? worksheet?.Cells[row, MoldHeaders["Paint"]].Value?.ToString()?.Trim() : string.Empty;

                                if (!string.IsNullOrEmpty(mold.MoldCode) && mold.MoldAmount != 0)
                                {
                                    mold.ProjectId = CurrentProject.Id;
                                    molds.Add(mold);
                                }
                            }//

                            await dataHandler.AddMoldRange(molds);
                            LoadMoldData();
                            await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Molds added" }, "RootDialogHost");

                        }
                    }//

                }//
            }
            catch (Exception err)
            {
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = err.Message }, "RootDialogHost");
            }
        }//

        private async void showProgressReport()
        {
            var vm = new ProjectReportViewModel();
            vm.Label = $"{CurrentProject.JobName} progress report";
            vm.ProjectReports = new(await dataHandler.GetProjectsReports(1, CurrentProject.Id));
            var win = new ViewProjectReport(vm);
            win.ShowDialog();
        }

        private static void getImgDimensions(string imgPath, ref int width, ref int height, int w = 150, int h = 150)
        {
            var img = new Bitmap(imgPath);
            int sourceWidth = img.Width;
            int sourceHeight = img.Height;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            if (sourceWidth < w && sourceHeight < h)
            {
                width = sourceWidth + 5;
                height = sourceHeight + 5;
                return;
            }

            nPercentW = ((float)w / (float)sourceWidth);
            nPercentH = ((float)h / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((w - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((h - (sourceHeight * nPercent)) / 2);
            }

            width = (int)(sourceWidth * nPercent);
            height = (int)(sourceHeight * nPercent);
        }//

        private async void printReport()
        {
            using (PdfDocument document = new PdfDocument())
            {
                PdfPage page = document.Pages.Add();
                page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);

                PdfFont font = new PdfTrueTypeFont(new Font("Arial", 28, System.Drawing.FontStyle.Bold));
                PdfFont textFont = new PdfTrueTypeFont(new Font("Arial", 20, System.Drawing.FontStyle.Bold));
                PdfFont textFont3 = new PdfTrueTypeFont(new Font("Arial", 12, System.Drawing.FontStyle.Underline | System.Drawing.FontStyle.Bold));

                var currentPageCount = 2;
                var size = page.GetClientSize().Width / 2;

                PdfStringFormat format = new PdfStringFormat();
                format.Alignment = PdfTextAlignment.Center;

                PdfTextElement textElement;
                PdfLayoutFormat layoutFormat = new PdfLayoutFormat();
                layoutFormat.Layout = PdfLayoutType.Paginate;
                layoutFormat.Break = PdfLayoutBreakType.FitElement;

                const int paragraphGap = 5;

                var height = page.GetClientSize().Height - 130;

                var pallets = await dataHandler.GetFilledPallets(1, CurrentProject.Id);
                var phone = "Phone: (201) 350-5111";
                var mail = "  Email: estimating@masterpieceprecast.com";
                var addr = $"{CurrentProject.Address}";
                var stoneType = "Cast stone";
                var pHeight = 5.0f;

                var index = 1;
                foreach (var pallet in pallets)
                {
                    PdfGraphics graphics = page.Graphics;
                    int destWidth = 0;
                    int destHeight = 0;
                    getImgDimensions(Path.Combine(Directory.GetCurrentDirectory(), "Images", "logo.png"), ref destWidth, ref destHeight, 121, 74);
                    FileStream imageStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "Images", "logo.png"), FileMode.Open, FileAccess.Read);
                    PdfBitmap image = new PdfBitmap(imageStream);

                    graphics.DrawImage(image, new PointF { X = size - (destWidth / 2), Y = pHeight }, new SizeF { Height = destHeight, Width = destWidth });

                    pHeight += 10;
                    textElement = new PdfTextElement(phone, textFont3, null, PdfBrushes.Black, format);
                    var result = textElement.Draw(page, new PointF(140, pHeight + destHeight), layoutFormat);

                    textElement = new PdfTextElement(mail, textFont3, PdfBrushes.Blue);
                    result = textElement.Draw(page, new PointF(result.Bounds.X + result.Bounds.Width, pHeight + destHeight), layoutFormat);

                    pHeight += (result.Bounds.Height + 100);
                    textElement = new PdfTextElement(addr, font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);

                    pHeight += (result.Bounds.Height + 10);
                    textElement = new PdfTextElement($"#{index}", font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);

                    pHeight += (result.Bounds.Height + 5);
                    textElement = new PdfTextElement(stoneType, font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);

                    pHeight += 10;

                    var mCodes = pallet.MoldCodes.Split(",").ToList().Order();
                    var grouped = mCodes.GroupBy(s => s).Select(g => new { Symbol = g.Key, Count = g.Count() });

                    var types = string.Empty;
                    foreach (var item in grouped)
                        types += $"{item.Count} {item.Symbol} ";

                    pHeight += (result.Bounds.Height + 5);
                    textElement = new PdfTextElement(types, textFont, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width, page.GetClientSize().Height), layoutFormat);
                    pHeight = result.Bounds.Y + 75;

                    ///...........................
                    ///
                    graphics.DrawImage(image, new PointF { X = size - (destWidth / 2), Y = pHeight }, new SizeF { Height = destHeight, Width = destWidth });

                    pHeight += 10;
                    textElement = new PdfTextElement(phone, textFont3, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new PointF(140, pHeight + destHeight), layoutFormat);

                    textElement = new PdfTextElement(mail, textFont3, PdfBrushes.Blue);
                    result = textElement.Draw(page, new PointF(result.Bounds.X + result.Bounds.Width, pHeight + destHeight), layoutFormat);

                    pHeight += (result.Bounds.Height + 100);
                    textElement = new PdfTextElement(addr, font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);

                    pHeight += (result.Bounds.Height + 10);
                    textElement = new PdfTextElement($"#{index}", font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);

                    pHeight += (result.Bounds.Height + 5);
                    textElement = new PdfTextElement(stoneType, font, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width - 15, page.GetClientSize().Height), layoutFormat);
                    pHeight += 10;

                    pHeight += (result.Bounds.Height + 5);
                    textElement = new PdfTextElement(types, textFont, null, PdfBrushes.Black, format);
                    result = textElement.Draw(page, new RectangleF(5, pHeight, page.GetClientSize().Width, page.GetClientSize().Height), layoutFormat);

                    ++index;
                    if (index <= pallets.Count)
                    {
                        pHeight = 5.0f;
                        page = document.Pages.Add();
                        page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
                    }
                }//

                MemoryStream stream = new MemoryStream();
                document.Save(stream);
                stream.Position = 0;

                PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
                var printDocumentWin = new PrintDocumentWin();
                printDocumentWin.pdfViewer.Load(pdfLoadedDocument);
                printDocumentWin.ShowDialog();
            }
        }

        private void filledPalletCodes()
        {
            PdfDocument doc = new PdfDocument();
            PdfPage page = doc.Pages.Add();
            page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
            PdfFont textFont = new PdfTrueTypeFont(new Font("Arial", 13, System.Drawing.FontStyle.Regular));

            var col = 5.0f;
            var row = 5.0f;

            PdfUnitConvertor convertor = new PdfUnitConvertor();
            float height = convertor.ConvertUnits(1.69f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            float width = convertor.ConvertUnits(6.28f, PdfGraphicsUnit.Centimeter, PdfGraphicsUnit.Point);
            PdfStringFormat format = new PdfStringFormat();
            format.Alignment = PdfTextAlignment.Center;

            var barcode = new PdfCode128BBarcode();
            barcode.Size = new SizeF(width - 10, height);
            barcode.Font = textFont;
            barcode.Text = $"{CurrentProject.JobName}:PF";

            for (int i = 0; i < 3; ++i)
            {
                barcode.Draw(page, new PointF(col, row));
                col += width;
                if (col >= (page.GetClientSize().Width - 50))
                {
                    col = 5.0f;
                    row += 40 + (height * 2);

                    if (row >= (page.GetClientSize().Height - 50.0f))
                    {
                        row = 5.0f;
                        page = doc.Pages.Add();
                        page.Section.PageSettings.Size = new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            stream.Position = 0;
            doc.Close(true);

            PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
            var printDocumentWin = new PrintDocumentWin();
            printDocumentWin.pdfViewer.Load(pdfLoadedDocument);
            printDocumentWin.ShowDialog();
        }

        private void viewPallets()
        {
            var vm = new ViewPalletsViewModel();
            vm.LoadMoldData(CurrentProject.Id);
            var viewPalletWin = new ViewPallets(vm);
            viewPalletWin.ShowDialog();
        }

        private void goBack()
        {
            WeakReferenceMessenger.Default.Send(new ProjectShellNavModel { Data = "PShellNav-Project" });
        }

        private async void delteMoldData(MoldModel model)
        {
            try
            {
                await dataHandler.DeleteMold(model.Id);
                Molds.Remove(model);
                WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
            }
            catch (ArgumentNullException err)
            {
                await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = err.Message }, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private async void editMoldData(MoldModel model)
        {
            try
            {
                var vm = new AddMoldDialogViewModel();
                vm.IsEditing = true;
                vm.CurrentProject = CurrentProject;

                vm.Mold = new MoldModel();
                vm.Mold.Id = model.Id;
                vm.Mold.MoldAmount = model.MoldAmount;
                vm.Mold.MoldSize = model.MoldSize;
                vm.Mold.MoldCode = model.MoldCode;
                vm.Mold.MoldCut = model.MoldCut;
                vm.Mold.MoldPhoto = model.MoldPhoto;
                vm.Mold.MoldInfo = model.MoldInfo;
                vm.Mold.Cut = model.Cut;
                vm.Mold.Paint = model.Paint;
                vm.Mold.Project = model.Project;
                vm.Mold.ProjectId = model.ProjectId;

                var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            }
            catch (Exception err) { await MaterialDesignThemes.Wpf.DialogHost.Show(new DialogViewModel { Label = "Error occured. Refresh and try agian." }, "RootDialogHost"); }
        }

        private async void addMold()
        {
            var vm = new AddMoldDialogViewModel();
            vm.CurrentProject = CurrentProject;
            var result = await MaterialDesignThemes.Wpf.DialogHost.Show(vm, "RootDialogHost");
            WeakReferenceMessenger.Default.Send(new DataMessageModel { Data = "Mold-Reload" });
        }

        public void LoadMoldData()
        {
            Task.Run(async () =>
            {
                var notes = dataHandler.context.ProjectNotes.Where(obj => obj.ProjectId == CurrentProject.Id);
                ProjectNotes = new ObservableCollection<ProjectNotesModels>(notes);

                var data = await dataHandler.GetMolds(1, CurrentProject.Id);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Molds = new(data);
                    var count = Molds.Sum(obj => obj.MoldAmount);
                    var qty = Molds.Sum(obj => obj?.ProcessCompleteCount);
                    if (count != 0 && qty != 0)
                        PercentageCleared = Math.Round((((double)qty / (double)count) * 100.0), 2);
                });
            });
        }

        private void recieveViewData(DataMessageModel model)
        {
            if (model.Data == "Mold-Reload")
            {
                dataHandler = new DataHandler.DataHandler();
                LoadMoldData();
            }

            else if (model.Data == "Settings-Updated")
            {
                dataHandler = new DataHandler.DataHandler();
                LoadMoldData();
            }
        }//

    }
}
