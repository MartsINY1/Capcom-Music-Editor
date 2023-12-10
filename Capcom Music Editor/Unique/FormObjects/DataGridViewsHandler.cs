using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using System.Drawing;
using Mega_Music_Editor.Unique.MusicSheetsInstructionsDatas;
using Mega_Music_Editor.Reusable;

namespace Mega_Music_Editor.Unique
{
    enum RdButtonSheetType
    {
        Music = 0,
        Loop = 1,
        Break = 2,
    }

    public static class ParameterPasser
    {
        public static string flagValue = "";
    }

    partial class DataGridViewsHandler
    {
        static private GameType _GameType;
        static private bool _UsePianoKeysForNotes = false;

        // Constant
        private readonly int columnOffset = 1;   // Quantity of columns that are always there no matter the type (line position, etc...)
        private readonly int maxQtyChannels = 8;

        // One Form is linked to this handler forever. Set in construction
        private readonly Form _currentForm = null;

        // Data grids quantity varies -> Contained in Lists
        private readonly List<DataGridView> listMusicDataGridView = null;
        private readonly List<DataGridView> listLoopDataGridView = null;
        private readonly List<DataGridView> listBreakDataGridView = null;
        private readonly List<int> listMusicDataGridViewScrollBarPos = null;
        private readonly List<int> listLoopDataGridViewScrollBarPos = null;
        private readonly List<int> listBreakDataGridViewScrollBarPos = null;


        private int _lastChannelSelected = 0;
        private RdButtonSheetType _lastSheetSelected = RdButtonSheetType.Music;


        private readonly ContextMenu dgvContextMenu = new ContextMenu();
        private readonly ContextMenu dgvContextMenuWhenNoCell = new ContextMenu();


        private int highestSelectionRow = 0;
        private int lowestSelectionRow = 0;
        private int indexOfChannelOnContextMenuPopup;



        /// <summary>
        /// Constructor to link view
        /// </summary>
        /// <param name="currentForm"></param>
        public DataGridViewsHandler(Form currentForm, GameType gameType, ref GroupBox gbxSheetChoice, ref GroupBox gbxConsoleChoice)
        {
            // Link the given form to the current object
            _currentForm = currentForm;

            // Create DataGridViews
            listMusicDataGridView = new List<DataGridView>();
            listLoopDataGridView = new List<DataGridView>();
            listBreakDataGridView = new List<DataGridView>();
            listMusicDataGridViewScrollBarPos = new List<int>();
            listLoopDataGridViewScrollBarPos = new List<int>();
            listBreakDataGridViewScrollBarPos = new List<int>();
            
            // Create grids for 8 channels
            for (int i = 0; i < maxQtyChannels; i++)
            {
                AddOneDataGridToList(ref listMusicDataGridView, i, PageName.Notes, GameType.NesA);
                AddOneDataGridToList(ref listLoopDataGridView, i, PageName.Loops, GameType.NesA);
                AddOneDataGridToList(ref listBreakDataGridView, i, PageName.Breaks, GameType.NesA);

                // List to contain position of scroll bar for each
                listMusicDataGridViewScrollBarPos.Add(0);
                listLoopDataGridViewScrollBarPos.Add(0);
                listBreakDataGridViewScrollBarPos.Add(0);
            }

            // Hide all sheets
            HideAllSheet();
            ShowOneSheet(0, RdButtonSheetType.Music);

            // Create the groupbox to pick a channel
            CreateGroupBoxForChannelSelection(ref gbxSheetChoice, ref gbxConsoleChoice);

            SetGameType(gameType);


            dgvContextMenu.MenuItems.Add("Add one line before selection", MenuItemAddOneLineBeforeSelection_Click);
            dgvContextMenu.MenuItems.Add("Add one line after selection", MenuItemAddOneLineAfterSelection_Click);
            dgvContextMenu.MenuItems.Add("Add lines before selection", MenuItemAddLinesBeforeSelection_Click);
            dgvContextMenu.MenuItems.Add("Add lines after selection", MenuItemAddLinesAfterSelection_Click);
            dgvContextMenu.MenuItems.Add("Not implemented - Copy Selection", MenuItemCopySelection_Click);
            dgvContextMenu.MenuItems.Add("Not implemented - Paste Selection", MenuItemPaste_Click);
            dgvContextMenu.MenuItems.Add("Delete selected lines", MenuItemDeleteLinesSelected_Click);


            dgvContextMenuWhenNoCell.MenuItems.Add("Add one line", MenuItemAddOneLine_Click);
            dgvContextMenuWhenNoCell.MenuItems.Add("Add lines", MenuItemAddLines_Click);


            ChangeDisplayOfConsoleSpecificColumns(gameType);
        }

        private void SetEmptyLineValue(int line)
        {
            string cellEmptyValue = "";

            for (int i = 1; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].ColumnCount; i++)
            {
                cellEmptyValue = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(i - columnOffset, PageName.Notes)._ColumnDefaultValue;

                String[] cellEmptyValueArray = cellEmptyValue.Split('|');

                for (int z = 0; z < cellEmptyValueArray.Length; z++)
                {
                    listMusicDataGridView[indexOfChannelOnContextMenuPopup][i + z, line].Value = cellEmptyValueArray[z];
                }
            }

            for (int i = 1; i < listLoopDataGridView[indexOfChannelOnContextMenuPopup].ColumnCount; i++)
            {
                int columnToSkip = 0;

                cellEmptyValue = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(i - columnOffset, PageName.Loops)._ColumnDefaultValue;

                String[] cellEmptyValueArray = cellEmptyValue.Split('|');

                for (int z = 0; z < cellEmptyValueArray.Length; z++)
                {
                    listLoopDataGridView[indexOfChannelOnContextMenuPopup][i + z, line].Value = cellEmptyValueArray[z];
                    columnToSkip++;
                }

                i += columnToSkip - 1;
            }

            for (int i = 1; i < listBreakDataGridView[indexOfChannelOnContextMenuPopup].ColumnCount; i++)
            {
                int columnToSkip = 0;

                cellEmptyValue = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(i - columnOffset, PageName.Breaks)._ColumnDefaultValue;

                String[] cellEmptyValueArray = cellEmptyValue.Split('|');

                for (int z = 0; z < cellEmptyValueArray.Length; z++)
                {
                    listBreakDataGridView[indexOfChannelOnContextMenuPopup][i + z, line].Value = cellEmptyValueArray[z];
                    columnToSkip++;
                }

                i += columnToSkip - 1;
            }
        }

        private void AddOneLineCurrentSelectedChannel(int positionToAdd)
        {
            listMusicDataGridView[indexOfChannelOnContextMenuPopup].Rows.Insert(positionToAdd, new DataGridViewRow());
            listLoopDataGridView[indexOfChannelOnContextMenuPopup].Rows.Insert(positionToAdd, new DataGridViewRow());
            listBreakDataGridView[indexOfChannelOnContextMenuPopup].Rows.Insert(positionToAdd, new DataGridViewRow());

            SetEmptyLineValue(positionToAdd);
        }
        private void DeleteOneLineCurrentSelectSheet(int positionToDelete)
        {
            listMusicDataGridView[indexOfChannelOnContextMenuPopup].Rows.RemoveAt(positionToDelete);
            listLoopDataGridView[indexOfChannelOnContextMenuPopup].Rows.RemoveAt(positionToDelete);
            listBreakDataGridView[indexOfChannelOnContextMenuPopup].Rows.RemoveAt(positionToDelete);
        }
        private void LineNumbering(int position)
        {
            listMusicDataGridView[indexOfChannelOnContextMenuPopup].Rows[position].Cells[0].Value = position;
            listLoopDataGridView[indexOfChannelOnContextMenuPopup].Rows[position].Cells[0].Value = position;
            listBreakDataGridView[indexOfChannelOnContextMenuPopup].Rows[position].Cells[0].Value = position;
        }

        

        private void MenuItemAddOneLineBeforeSelection_Click(Object sender, System.EventArgs e)
        {
            AddOneLineCurrentSelectedChannel(lowestSelectionRow);
            
            // Renumber every line
            for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
            {
                LineNumbering(i);

                CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, 1, true);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break1, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break2, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break3, 1);
                CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break4, 1);
            }

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }
        private void MenuItemAddOneLine_Click(Object sender, System.EventArgs e)
        {
            AddOneLineCurrentSelectedChannel(0);


            LineNumbering(0);

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }
        private void MenuItemAddOneLineAfterSelection_Click(Object sender, System.EventArgs e)
        {
            AddOneLineCurrentSelectedChannel(highestSelectionRow + 1);
            
            // Renumber every line
            for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
            {
                LineNumbering(i);

                CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, 1, true);
                CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break1, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break2, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break3, 1);
                CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break4, 1);
            }

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }

        private void MenuItemAddLines_Click(Object sender, System.EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Number of lines to add", "Prompt", "1", 0, 0);
            int qtyLinesToAdd = 0;

            int.TryParse(input, out qtyLinesToAdd);

            if (qtyLinesToAdd > 0)
            {
                for (int i = 0; i < qtyLinesToAdd; i++)
                {
                    AddOneLineCurrentSelectedChannel(0);
                }
                // Number every line
                for (int i = 0; i < qtyLinesToAdd; i++)
                {
                    LineNumbering(i);
                }
            }
            else
            {
                MessageBox.Show("Number of lines to add is invalid.");
            }
            
            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }

        private void MenuItemAddLinesBeforeSelection_Click(Object sender, System.EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Number of lines to add", "Prompt", "1", 0, 0);
            int qtyLinesToAdd = 0;

            int.TryParse(input, out qtyLinesToAdd);

            if (qtyLinesToAdd > 0)
            {
                for (int i = 0; i < qtyLinesToAdd; i++)
                {
                    AddOneLineCurrentSelectedChannel(lowestSelectionRow);
                }
                // Renumber every line
                for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
                {
                    LineNumbering(i);

                    CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, qtyLinesToAdd, true);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break1, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break2, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break3, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionBeforeLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break4, qtyLinesToAdd);
                }
            }
            else
            {
                MessageBox.Show("Number of lines to add is invalid.");
            }

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }
        private void MenuItemAddLinesAfterSelection_Click(Object sender, System.EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Number of lines to add", "Prompt", "1", 0, 0);
            int qtyLinesToAdd = 0;

            int.TryParse(input, out qtyLinesToAdd);

            if (qtyLinesToAdd > 0)
            {
                for (int i = 0; i < qtyLinesToAdd; i++)
                {
                    AddOneLineCurrentSelectedChannel(highestSelectionRow + 1);
                }


                // Renumber every line
                for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
                {
                    LineNumbering(i);

                    CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, qtyLinesToAdd, true);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break1, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break2, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break3, qtyLinesToAdd);
                    CorrectRowLineNumbersAfterAdditionAfterLine(listBreakDataGridView, i, ColumnsPosition.NesA.Break4, qtyLinesToAdd);
                }
            }
            else
            {
                MessageBox.Show("Number of lines to add is invalid.");
            }

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }
        private void MenuItemCopySelection_Click(Object sender, System.EventArgs e)
        {
            // Code to insert Zettabit
        }
        private void MenuItemPaste_Click(Object sender, System.EventArgs e)
        {
            // Code to insert Zettabit
        }

        public void ConvertNoiseChannelToNormalChannel()
        {
            int qtyLine = listMusicDataGridView[3].RowCount;
            string currentNote = "";
            string noteHexCode = "";
            string noteDatas = "";

            for (int i = 0; i < qtyLine; i++)
            {
                currentNote = listMusicDataGridView[3].Rows[i].Cells[1].Value.ToString();

                if (currentNote != "---" && currentNote != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                {
                    noteHexCode = GetMusicEngineNotesDatas.GetDatasByNoiseNote(currentNote)._HexCode;
                    noteDatas = GetMusicEngineNotesDatas.GetDatasByHexCodes(noteHexCode)._Name;

                    listMusicDataGridView[3].Rows[i].Cells[1].Value = noteDatas;
                }
            }
        }

        // We need to store those values in minimise because for some reason they are reset
        public void StoreScrollBarsPositions()
        {
            for (int i = 0; i < maxQtyChannels; i++)
            {
                if (listMusicDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listMusicDataGridViewScrollBarPos[i] = listMusicDataGridView[i].FirstDisplayedScrollingRowIndex;
                if (listLoopDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listLoopDataGridViewScrollBarPos[i] = listLoopDataGridView[i].FirstDisplayedScrollingRowIndex;
                if (listBreakDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listBreakDataGridViewScrollBarPos[i] = listBreakDataGridView[i].FirstDisplayedScrollingRowIndex;
            }

        }
        public void RecuperateScrollBarsPositions()
        {
            for (int i = 0; i < maxQtyChannels; i++)
            {
                if (listMusicDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listMusicDataGridView[i].FirstDisplayedScrollingRowIndex = listMusicDataGridViewScrollBarPos[i];
                if (listLoopDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listLoopDataGridView[i].FirstDisplayedScrollingRowIndex = listLoopDataGridViewScrollBarPos[i];
                if (listBreakDataGridView[i].FirstDisplayedScrollingRowIndex != -1) listBreakDataGridView[i].FirstDisplayedScrollingRowIndex = listBreakDataGridViewScrollBarPos[i];
            }

        }

        public int ReturnCellValueAsInteger(DataGridViewCell cellValue)
        {
            int tempInt = -1;
            double tempDouble = 0;
            string cellValueStr = "";
            
            cellValueStr = cellValue.Value.ToString();

            if (Double.TryParse(cellValueStr, out tempDouble))
            {
                tempInt = Int32.Parse(cellValueStr);
            }

            return tempInt;
        }

        private void CorrectRowLineNumbersAfterDelete(List<DataGridView> lstDataGridView, int rowIndex, int columnIndex, int qtyLinesToDelete, bool isJump = false)
        {
            int lineValue = 0;
            if (!isJump)
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1]);
            }
            else
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex]);
            }

            if (lineValue >= 0)
            {
                // If line is one with a new number
                if (lineValue >= lowestSelectionRow && lineValue <= highestSelectionRow)
                {
                    string tempStr = "";

                    // Since jump and loop have different values in their cell, we just change the value by a string of - the same lenght of current string
                    for (int i = 0; i < lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex].Value.ToString().Count(); i++)
                    {
                        tempStr += "-";
                    }
                    lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex].Value = tempStr;

                    if (!isJump)
                    {
                        tempStr = "";

                        // Since jump and loop have different values in their cell, we just change the value by a string of - the same lenght of current string
                        for (int i = 0; i < lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1].Value.ToString().Count(); i++)
                        {
                            tempStr += "-";
                        }
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1].Value = tempStr;
                    }
                }
                else if (lineValue > highestSelectionRow)
                {
                    if (!isJump)
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1].Value = (lineValue - qtyLinesToDelete).ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex].Value = (lineValue - qtyLinesToDelete).ToString().PadLeft(5, '0');
                    }
                }
            }
        }


        private void CorrectRowLineNumbersAfterAdditionBeforeLine(List<DataGridView> lstDataGridView, int rowIndex, int columnIndex, int qtyLinesAdded, bool isJump = false)
        {
            int lineValue = 0;
            if (!isJump)
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1]);
            }
            else
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex]);
            }

            if (lineValue >= 0)
            {
                // If line is one with a new number
                if (lineValue >= lowestSelectionRow)
                {
                    if (!isJump)
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1].Value = (lineValue + qtyLinesAdded).ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex].Value = (lineValue + qtyLinesAdded).ToString().PadLeft(5, '0');
                    }
                }
            }
        }


        private void CorrectRowLineNumbersAfterAdditionAfterLine(List<DataGridView> lstDataGridView, int rowIndex, int columnIndex, int qtyLinesAdded, bool isJump = false)
        {
            int lineValue = 0;
            if (!isJump)
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1]);
            }
            else
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex]);
            }

            if (lineValue >= 0)
            {
                // If line is one with a new number
                if (lineValue > lowestSelectionRow)
                {
                    if (!isJump)
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1].Value = (lineValue + qtyLinesAdded).ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex].Value = (lineValue + qtyLinesAdded).ToString().PadLeft(5, '0');
                    }
                }
            }
        }

        private bool CheckIfDeletedLineIsReferred(List<DataGridView> lstDataGridView, int rowIndex, int columnIndex, bool isJump = false)
        {
            int lineValue = 0;
            if (!isJump)
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex + 1]);
            }
            else
            {
                lineValue = ReturnCellValueAsInteger(lstDataGridView[indexOfChannelOnContextMenuPopup].Rows[rowIndex].Cells[columnOffset + columnIndex]);
            }

            if (lineValue >= 0)
            {
                // If line is one with a new number
                if (lineValue >= lowestSelectionRow && lineValue <= highestSelectionRow)
                {
                    return true;
                }
            }

            return false;
        }

        private void MenuItemDeleteLinesSelected_Click(Object sender, EventArgs e)
        {
            bool oneLineToDeleteIsReferred = false;
            int lineWithReferredLine = 0;
            int qtyLinesToDelete = highestSelectionRow - lowestSelectionRow + 1;

            for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
            {
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, true);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Break1);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Break2);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Break3);
                oneLineToDeleteIsReferred |= CheckIfDeletedLineIsReferred(listLoopDataGridView, i, ColumnsPosition.NesA.Break4);

                if (oneLineToDeleteIsReferred)
                {
                    lineWithReferredLine = i;
                    break;
                }
            }

            if (oneLineToDeleteIsReferred)
            {
                DialogResult msgButton = MessageBox.Show("On channel " + indexOfChannelOnContextMenuPopup + " at line " + lineWithReferredLine + " a deleted line is referred." + Environment.NewLine + "Proceed?", "Delete lines", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (msgButton == DialogResult.No)
                {
                    return;
                }
            }

            for (int i = 0; i < qtyLinesToDelete; i++)
            {
                DeleteOneLineCurrentSelectSheet(lowestSelectionRow);
            }

            // If first line with new number isn't out of bound
            if ((lowestSelectionRow) < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount)
            {
                // Renumber every line
                for (int i = 0; i < listMusicDataGridView[indexOfChannelOnContextMenuPopup].RowCount; i++)
                {
                    LineNumbering(i);

                    CorrectRowLineNumbersAfterDelete(listLoopDataGridView, i, ColumnsPosition.NesA.Loop1, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listLoopDataGridView, i, ColumnsPosition.NesA.Loop2, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listLoopDataGridView, i, ColumnsPosition.NesA.Loop3, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listLoopDataGridView, i, ColumnsPosition.NesA.Loop4, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listLoopDataGridView, i, ColumnsPosition.NesA.Jump, qtyLinesToDelete, true);
                    CorrectRowLineNumbersAfterDelete(listBreakDataGridView, i, ColumnsPosition.NesA.Break1, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listBreakDataGridView, i, ColumnsPosition.NesA.Break2, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listBreakDataGridView, i, ColumnsPosition.NesA.Break3, qtyLinesToDelete);
                    CorrectRowLineNumbersAfterDelete(listBreakDataGridView, i, ColumnsPosition.NesA.Break4, qtyLinesToDelete);
                }
            }

            ColorLineOfDataGridViews(indexOfChannelOnContextMenuPopup);
        }


        private bool ReturnNoteInZeroToTwoValue(int octave, ref string noteIn, out string noteOut)
        {
            string temp = "";
            int currentOct = 0;

            // Needs to be set
            noteOut = noteIn;

            if (noteIn == MusicEngineFixedNotesDatas.Names.NesA.Delay)
            {
                return true;
            }
            if (noteIn == MusicEngineFixedNotesDatas.Names.NesA.Skip)
            {
                return true;
            }
            
            temp = noteIn.Substring(0, 2);

            currentOct = Convert.ToInt32(noteIn.Substring(2, 1));

            currentOct -= octave;

            // If digit of note minus octave return <= 2, it means value is playable, and need to be >= 0
            if (currentOct <= 2 && currentOct >= 0)
            {
                noteOut = temp + currentOct.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ColorLineOfDataGridViews(int channel)
        {
            int loopCount = 0;

            foreach (DataGridViewRow row in listMusicDataGridView[channel].Rows)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 255, 0);
                row.DefaultCellStyle.BackColor = Color.Black;

                if (loopCount % 4 == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                }
                if (loopCount % 16 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(100, 100, 100);
                }
                loopCount++;
            }

            loopCount = 0;

            foreach (DataGridViewRow row in listLoopDataGridView[channel].Rows)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 255, 0);
                row.DefaultCellStyle.BackColor = Color.Black;

                if (loopCount % 4 == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                }
                if (loopCount % 16 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(100, 100, 100);
                }
                loopCount++;
            }

            loopCount = 0;

            foreach (DataGridViewRow row in listBreakDataGridView[channel].Rows)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(0, 255, 0);
                row.DefaultCellStyle.BackColor = Color.Black;

                if (loopCount % 4 == 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Yellow;
                    row.DefaultCellStyle.BackColor = Color.FromArgb(40, 40, 40);
                }
                if (loopCount % 16 == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(100, 100, 100);
                }
                loopCount++;
            }
        }

        private void WriteLineInDataGridView(int rowIndex, int channel, MusicLine currentLine)
        {
            int tempColumnPos = 0;
            string splitter = "";

            listMusicDataGridView[channel].Rows[rowIndex].Cells[0].Value = (rowIndex).ToString();
            listLoopDataGridView[channel].Rows[rowIndex].Cells[0].Value = (rowIndex).ToString();
            listBreakDataGridView[channel].Rows[rowIndex].Cells[0].Value = (rowIndex).ToString();


            // Note handling
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Note.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Note.GetNote();

                // Sometime, with connect handling I made, which is messy, it seems a note is active while it's a connection from
                // a previous note, and this code here will write empty in the note. This shouldn't be:
                if (currentLine._Note.GetNote() == "") listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = "---";
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue;
            }
            // Volume handling
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Volume)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Volume.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Volume.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Volume)._ColumnDefaultValue;
            }
            // Instrument handling
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Instrument)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Instrument.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Instrument.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Instrument)._ColumnDefaultValue;
            }
            // Octave handling
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Octave)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Octave.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Octave.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Octave)._ColumnDefaultValue;
            }
            // Tone Lenght
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._ToneLenght.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._ToneLenght.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght)._ColumnDefaultValue;
            }
            // Tune Pitch
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TunePitch)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._TunePitch.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._TunePitch.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TunePitch)._ColumnDefaultValue;
            }
            // Pitch Slide
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlide)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._PitchSlide.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._PitchSlide.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlide)._ColumnDefaultValue;
            }
            // Tone Type
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneType)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._ToneType.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._ToneType.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneType)._ColumnDefaultValue;
            }
            // Speed
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Speed)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Speed.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Speed.GetHighValue();
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Speed.GetLowValue();
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Speed)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Transpose
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Transpose)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Transpose.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Transpose.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Transpose)._ColumnDefaultValue;
            }
            // Global Transpose
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalTranspose)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._GlobalTranspose.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._GlobalTranspose.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalTranspose)._ColumnDefaultValue;
            }
            // Set Flag
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Flags)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Flag.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Flag.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Flags)._ColumnDefaultValue;
            }
            // Octave Plus
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.OctavePlus)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._OctavePlus.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = "TOG";
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.OctavePlus)._ColumnDefaultValue;
            }
            // Connect
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Connect)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Connect.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = "TOG";
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Connect)._ColumnDefaultValue;
            }
            // Triplet
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Triplet)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Triplet.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = "TOG";
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Triplet)._ColumnDefaultValue;
            }
            // Panning
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Panning)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Panning.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Panning.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Panning)._ColumnDefaultValue;
            }
            // Pitch Vibrato Depth
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoDepth)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._VibratoDept.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._VibratoDept.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoDepth)._ColumnDefaultValue;
            }
            // Tremolo Depth
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TremoloVolume)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._TremoloVolume.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._TremoloVolume.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TremoloVolume)._ColumnDefaultValue;
            }
            // Vibrato Tremolo Speed
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoOrTremolFrequency)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._VibratoOrTremoloFrequency.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._VibratoOrTremoloFrequency.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoOrTremolFrequency)._ColumnDefaultValue;
            }
            // 1A03
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ins1A03)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._ins1A03.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._ins1A03.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ins1A03)._ColumnDefaultValue;
            }
            // Pitch Slide Unknown
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlideUnk)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._PitchSlideUnk.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._PitchSlideUnk.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlideUnk)._ColumnDefaultValue;
            }
            // Unknown 1B
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.SfxToggle)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._SfxToggle.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._SfxToggle.GetHighValue();
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._SfxToggle.GetLowValue();
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.SfxToggle)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Sfx
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Sfx)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Sfx.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Sfx.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Sfx)._ColumnDefaultValue;
            }
            // Toggle Full Note Lenght
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght2)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._ToneLength2.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._ToneLength2.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght2)._ColumnDefaultValue;
            }
            // Panning Movement Speed
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PannMovementSpeed)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._PanningMovementSpeed.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._PanningMovementSpeed.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PannMovementSpeed)._ColumnDefaultValue;
            }
            // Unknown1F
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Unknown1F)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._unknown1F.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._unknown1F.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Unknown1F)._ColumnDefaultValue;
            }
            // Global Volume
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalVolume)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._GlobalVolume.IsActive())
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._GlobalVolume.GetValue();
            }
            else
            {
                listMusicDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalVolume)._ColumnDefaultValue;
            }


            // Loops Sheet
            // Loop1
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop1)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Loop1.IsActive())
            {
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Loop1.GetNumberOfLoops().ToString().PadLeft(2, '0');
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Loop1.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop1)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Loop2
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop2)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Loop2.IsActive())
            {
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Loop2.GetNumberOfLoops().ToString().PadLeft(2, '0');
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Loop2.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop2)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Loop3
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop3)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Loop3.IsActive())
            {
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Loop3.GetNumberOfLoops().ToString().PadLeft(2, '0');
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Loop3.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop3)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Loop4
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop4)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Loop4.IsActive())
            {
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Loop4.GetNumberOfLoops().ToString().PadLeft(2, '0');
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Loop4.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop4)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Jump
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Jump)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Jump.IsActive())
            {
                listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Jump.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Jump)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listLoopDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }



            // Breaks Sheet
            // Break1
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break1)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Break1.IsActive())
            {
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Break1.GetFlag();
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Break1.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break1)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Break2
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break2)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Break2.IsActive())
            {
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Break2.GetFlag();
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Break2.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break2)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Break3
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break3)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Break3.IsActive())
            {
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Break3.GetFlag();
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Break3.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break3)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
            // Break4
            tempColumnPos = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break4)._ColumnPosition;
            tempColumnPos += columnOffset;
            if (currentLine._Break4.IsActive())
            {
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos].Value = currentLine._Break4.GetFlag();
                listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + 1].Value = currentLine._Break4.GetLine().ToString().PadLeft(5, '0');
            }
            else
            {
                splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break4)._ColumnDefaultValue; ;

                String[] allDefaultValues = splitter.Split('|');

                for (int z = 0; z < allDefaultValues.Length; z++)
                {
                    listBreakDataGridView[channel].Rows[rowIndex].Cells[tempColumnPos + z].Value = allDefaultValues[z];
                }
            }
        }

        public GameType GetGameType()
        {
            return _GameType;
        }
        public bool GetUsePianoKeysForNotes()
        {
            return _UsePianoKeysForNotes;
        }

        private void SetGameType(GameType gameType)
        {
            _GameType = gameType;
        }
        public void SetUsePianoKeysForNotes(bool usePianoKeysForNotes)
        {
            _UsePianoKeysForNotes = usePianoKeysForNotes;
        }

        public void ChangeGameType(GameType gameType, ref GroupBox gbxSheetSelection, ref GroupBox gbxConsoleSelection)
        {
            if (_GameType != gameType)
            {
                SetGameType(gameType);

                // We recreate the grid and the group box with radio button to toggle through channels
                CreateGroupBoxForChannelSelection(ref gbxSheetSelection, ref gbxConsoleSelection);

                // Hide/Display appropriate columns
                ChangeDisplayOfConsoleSpecificColumns(gameType);
            }
        }

        public void ChangeDisplayOfAdvancedColumns(bool display)
        {
            // For the position of culumns, we need to do + 1 because the one with line number isn't counted
            if (display)
            {
                for (int i = 0; i < maxQtyChannels; i++)
                {
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Triplet + 1].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Connect + 1].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.OctavePlus + 1].Visible = true;
                }
            }
            else
            {
                for (int i = 0; i < maxQtyChannels; i++)
                {
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Triplet + 1].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Connect + 1].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.OctavePlus + 1].Visible = false;
                }
            }
        }
        public void ChangeDataGridViewSizes(int frmWidth, int frmHeight)
        {
            int widthOfDataGrid = frmWidth - 40;
            int heightOfDataGrid = frmHeight - 230;

            if (widthOfDataGrid < 100) widthOfDataGrid = 100;
            if (widthOfDataGrid > 1132) widthOfDataGrid = 1132; // No need to make it to big
            if (widthOfDataGrid < 100) widthOfDataGrid = 100;

            for (int i = 0; i < maxQtyChannels; i++)
            {
                listMusicDataGridView[i].Width = widthOfDataGrid;
                listLoopDataGridView[i].Width = widthOfDataGrid;
                listBreakDataGridView[i].Width = widthOfDataGrid;

                listMusicDataGridView[i].Height = heightOfDataGrid;
                listLoopDataGridView[i].Height = heightOfDataGrid;
                listBreakDataGridView[i].Height = heightOfDataGrid;
            }
        }

        public void ChangeDisplayOfConsoleSpecificColumns(GameType gameType)
        {
            if (gameType == GameType.NesA)
            {
                for (int i = 0; i < maxQtyChannels; i++)
                {
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ToneType + columnOffset].Visible = true;

                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Panning + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.VibratoDepth + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.TremoloVolume + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.VibratoOrTremolFrequency + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ins1A03 + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.PitchSlideUnk + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.SfxToggle + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Sfx + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ToneLenght2 + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.PannMovementSpeed + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Unknown1F + columnOffset].Visible = false;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.GlobalVolume + columnOffset].Visible = false;
                }
            }
            else if (gameType == GameType.SnesA)
            {
                for (int i = 0; i < maxQtyChannels; i++)
                {
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ToneType + columnOffset].Visible = false;
    
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Panning + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.VibratoDepth + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.TremoloVolume + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.VibratoOrTremolFrequency + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ins1A03 + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.PitchSlideUnk + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.SfxToggle + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Sfx + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.ToneLenght2 + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.PannMovementSpeed + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.Unknown1F + columnOffset].Visible = true;
                    listMusicDataGridView[i].Columns[ColumnsPosition.NesA.GlobalVolume + columnOffset].Visible = true;
                }
            }
        }

        private string ReturnNumberPressed(KeyEventArgs e)
        {
            string letterPressed = "";

            switch (e.KeyCode)
            {
                case Keys.D0:
                    {
                        letterPressed = "0";
                        break;
                    }
                case Keys.D1:
                    {
                        letterPressed = "1";
                        break;
                    }
                case Keys.D2:
                    {
                        letterPressed = "2";
                        break;
                    }
                case Keys.D3:
                    {
                        letterPressed = "3";
                        break;
                    }
                case Keys.D4:
                    {
                        letterPressed = "4";
                        break;
                    }
                case Keys.D5:
                    {
                        letterPressed = "5";
                        break;
                    }
                case Keys.D6:
                    {
                        letterPressed = "6";
                        break;
                    }
                case Keys.D7:
                    {
                        letterPressed = "7";
                        break;
                    }
                case Keys.D8:
                    {
                        letterPressed = "8";
                        break;
                    }
                case Keys.D9:
                    {
                        letterPressed = "9";
                        break;
                    }
            }
            return letterPressed;
        }

        private string ReturnNoteLetterPressed(KeyEventArgs e, bool usePianoKeysForNotes)
        {
            string letterPressed = "";

            switch (e.KeyCode)
            {
                case Keys.A:
                    {
                        letterPressed = "A";
                        break;
                    }
                case Keys.B:
                    {
                        letterPressed = usePianoKeysForNotes ? "G" : "B";
                        break;
                    }
                case Keys.C:
                    {
                        letterPressed = usePianoKeysForNotes ? "E" : "C";
                        break;
                    }
                case Keys.D:
                    {
                        letterPressed = "D";
                        break;
                    }
                case Keys.E:
                    {
                        letterPressed = "E";
                        break;
                    }
                case Keys.F:
                    {
                        letterPressed = "F";
                        break;
                    }
                case Keys.G:
                    {
                        letterPressed = "G";
                        break;
                    }
                case Keys.M:
                    {
                        letterPressed = usePianoKeysForNotes ? "H" : "";
                        break;
                    }
                case Keys.N:
                    {
                        letterPressed = usePianoKeysForNotes ? "A" : "";
                        break;
                    }
                case Keys.V:
                    {
                        letterPressed = usePianoKeysForNotes ? "F" : "";
                        break;
                    }
                case Keys.X:
                    {
                        letterPressed = usePianoKeysForNotes ? "D" : "";
                        break;
                    }
                case Keys.Z:
                    {
                        letterPressed = usePianoKeysForNotes ? "C" : "";
                        break;
                    }
            }
            return letterPressed;
        }

        private string ReturnHexCodePressed(KeyEventArgs e)
        {
            string hexCodePressed = "";

            hexCodePressed = ReturnNumberPressed(e);

            if (hexCodePressed == "")
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        {
                            hexCodePressed = "A";
                            break;
                        }
                    case Keys.B:
                        {
                            hexCodePressed = "B";
                            break;
                        }
                    case Keys.C:
                        {
                            hexCodePressed = "C";
                            break;
                        }
                    case Keys.D:
                        {
                            hexCodePressed = "D";
                            break;
                        }
                    case Keys.E:
                        {
                            hexCodePressed = "E";
                            break;
                        }
                    case Keys.F:
                        {
                            hexCodePressed = "F";
                            break;
                        }
                }
            }
            return hexCodePressed;
        }

        private bool ReturnValueForNumericSlot(KeyEventArgs e, ref string cellValue, ref string futureCellValue, Hex maxValue, string futureCellValueIfDeleted)
        {
            string letterPressed = "";

            if (e.KeyCode == Keys.Enter)
            {
                futureCellValue = "00";
                e.Handled = true; // So normal Enter function isn't executed
                return true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                futureCellValue = futureCellValueIfDeleted;
                e.Handled = true; // So normal Enter function isn't executed
                return true;
            }
            else
            {
                if (!cellValue.Contains(futureCellValueIfDeleted))
                {
                        letterPressed = ReturnHexCodePressed(e);

                        if (letterPressed != "")
                        {
                            if (Control.ModifierKeys == Keys.Shift)
                            {
                                futureCellValue = letterPressed + cellValue.Substring(1, 1);
                            }
                            else
                            {
                                futureCellValue = cellValue.Substring(0, 1) + letterPressed;
                            }

                            if (new Hex(futureCellValue) <= maxValue)
                            {
                                return true;
                            }
                        }
                }
            }
            return false;
        }



        private bool ReturnValueForLineNumberSlot(KeyEventArgs e, ref string cellValue, ref string futureCellValue, string futureCellValueIfDeleted = null)
        {
            string digitPressed = "";

            if (e.KeyCode == Keys.Enter)
            {
                futureCellValue = "00000";
                e.Handled = true; // So normal Enter function isn't executed
                return true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                futureCellValue = futureCellValueIfDeleted;
                e.Handled = true; // So normal Enter function isn't executed
                return true;
            }
            else
            {
                if (cellValue != futureCellValueIfDeleted)
                {
                    if (cellValue != "")
                    {
                        digitPressed = ReturnNumberPressed(e);

                        if (digitPressed != "")
                        {
                            futureCellValue = cellValue + digitPressed;
                            futureCellValue = futureCellValue.Substring(1);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void MoveLineByIncrementsOfFour(KeyEventArgs e, DataGridView currentDgv)
        {

            var dgvCurrentCell = currentDgv.SelectedCells[0];

            if (e.KeyCode == Keys.P)
            {
                if (dgvCurrentCell.RowIndex - 4 < 0)
                    currentDgv.CurrentCell = currentDgv.Rows[0].Cells[dgvCurrentCell.ColumnIndex];
                else
                    currentDgv.CurrentCell = currentDgv.Rows[dgvCurrentCell.RowIndex - 4].Cells[dgvCurrentCell.ColumnIndex];
            }
            if (e.KeyCode == Keys.L)
            {
                if (dgvCurrentCell.RowIndex + 4 < currentDgv.RowCount)
                    currentDgv.CurrentCell = currentDgv.Rows[dgvCurrentCell.RowIndex + 4].Cells[dgvCurrentCell.ColumnIndex];
                else if (dgvCurrentCell.RowIndex + 3 < currentDgv.RowCount)
                    currentDgv.CurrentCell = currentDgv.Rows[dgvCurrentCell.RowIndex + 3].Cells[dgvCurrentCell.ColumnIndex];
                else if (dgvCurrentCell.RowIndex + 2 < currentDgv.RowCount)
                    currentDgv.CurrentCell = currentDgv.Rows[dgvCurrentCell.RowIndex + 2].Cells[dgvCurrentCell.ColumnIndex];
                else if (dgvCurrentCell.RowIndex + 1 < currentDgv.RowCount)
                    currentDgv.CurrentCell = currentDgv.Rows[dgvCurrentCell.RowIndex + 1].Cells[dgvCurrentCell.ColumnIndex];
            }
        }

        private void DgvMusicLoopBreak_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dgvCurrent = sender as DataGridView;

            if (dgvCurrent.RowCount == 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    dgvContextMenuWhenNoCell.Show(dgvCurrent, dgvCurrent.PointToClient(Cursor.Position));

                    
                    indexOfChannelOnContextMenuPopup = 0;

                    for (int i = 0; i < listMusicDataGridView.Count; i++)
                    {
                        if (listMusicDataGridView[i] == dgvCurrent || listLoopDataGridView[i] == dgvCurrent || listBreakDataGridView[i] == dgvCurrent)
                        {
                            indexOfChannelOnContextMenuPopup = i;
                            break;
                        }
                    }
                }
                return;
            }

            if (dgvCurrent.SelectedCells.Count == 0) return;

            var dgvMusicCurrentCell = dgvCurrent.SelectedCells[0];

            if (e.Button == MouseButtons.Right)
            {
                dgvContextMenu.Show(dgvCurrent, dgvCurrent.PointToClient(Cursor.Position));

                highestSelectionRow = dgvCurrent.SelectedCells[0].RowIndex;
                lowestSelectionRow = dgvCurrent.SelectedCells[0].RowIndex;

                foreach (DataGridViewCell selectedCell in dgvCurrent.SelectedCells)
                {
                    if (highestSelectionRow < selectedCell.RowIndex)
                    {
                        highestSelectionRow = selectedCell.RowIndex;
                    }

                    if (lowestSelectionRow > selectedCell.RowIndex)
                    {
                        lowestSelectionRow = selectedCell.RowIndex;
                    }
                }


                indexOfChannelOnContextMenuPopup = 0;

                for (int i = 0; i < listMusicDataGridView.Count; i++)
                {
                    if (listMusicDataGridView[i] == dgvCurrent || listLoopDataGridView[i] == dgvCurrent || listBreakDataGridView[i] == dgvCurrent)
                    {
                        indexOfChannelOnContextMenuPopup = i;
                        break;
                    }
                }
            }
        }
        
        private void DgvLoop_KeyDown(object sender, KeyEventArgs e)
        {
            int qtyCellsToEdit = 1; // 1 count for one
            DataGridView dgvLoop = sender as DataGridView;

            if (dgvLoop.RowCount == 0) return;

            // If delete, we delete content of all the cells
            if (e.KeyCode == Keys.Delete)
            {
                qtyCellsToEdit = dgvLoop.SelectedCells.Count;
            }

            for (int i = 0; i < qtyCellsToEdit; i++)
            {
                var dgvLoopCurrentCell = dgvLoop.SelectedCells[i];
                string cellValue = dgvLoopCurrentCell.Value.ToString();
                string futureCellValue = "";
                string splitter = "";

                MoveLineByIncrementsOfFour(e, dgvLoop);


                if ((dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop1 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop2 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop3 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop4 + columnOffset))
                {
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        dgvLoopCurrentCell.Value = futureCellValue;

                        if (!futureCellValue.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            if (dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex + 1].Value.ToString().Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex + 1].Value = "00000";
                            }
                        }
                        else
                        {
                            splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop1)._ColumnDefaultValue;

                            String[] allDefaultValues = splitter.Split('|');

                            for (int z = 0; z < allDefaultValues.Length; z++)
                            {
                                dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex + z].Value = allDefaultValues[z];
                            }
                        }
                    }

                }
                if ((dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop1 + 1 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop2 + 1 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop3 + 1 + columnOffset)
                    || (dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Loop4 + 1 + columnOffset))
                {
                    if (ReturnValueForLineNumberSlot(e, ref cellValue, ref futureCellValue, ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        dgvLoopCurrentCell.Value = futureCellValue;

                        if (!futureCellValue.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            if (dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex - 1].Value.ToString().Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex - 1].Value = "00";
                            }
                        }
                        else
                        {
                            splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Loop1)._ColumnDefaultValue;

                            String[] allDefaultValues = splitter.Split('|');

                            for (int z = 0; z < allDefaultValues.Length; z++)
                            {
                                dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex - 1 + z].Value = allDefaultValues[z];
                            }
                        }
                    }
                }

                if ((dgvLoopCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Jump + columnOffset))
                {
                    if (ReturnValueForLineNumberSlot(e, ref cellValue, ref futureCellValue, ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        dgvLoopCurrentCell.Value = futureCellValue;

                        if (futureCellValue.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Jump)._ColumnDefaultValue;

                            String[] allDefaultValues = splitter.Split('|');

                            for (int z = 0; z < allDefaultValues.Length; z++)
                            {
                                dgvLoop.Rows[dgvLoopCurrentCell.RowIndex].Cells[dgvLoopCurrentCell.ColumnIndex + z].Value = allDefaultValues[z];
                            }
                        }
                    }
                }
            }
        }

        private void DgvBreak_KeyDown(object sender, KeyEventArgs e)
        {
            int qtyCellsToEdit = 1; // 1 count for one
            DataGridView dgvBreak = sender as DataGridView;

            if (dgvBreak.RowCount == 0) return;

            // If delete, we delete content of all the cells
            if (e.KeyCode == Keys.Delete)
            {
                qtyCellsToEdit = dgvBreak.SelectedCells.Count;
            }

            for (int i = 0; i < qtyCellsToEdit; i++)
            {
                var dgvBreakCurrentCell = dgvBreak.SelectedCells[i];
                string cellValue = dgvBreakCurrentCell.Value.ToString();
                string futureCellValue = "";
                string splitter = "";

                MoveLineByIncrementsOfFour(e, dgvBreak);


                if ((dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break1 + columnOffset)
                    || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break2 + columnOffset)
                    || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break3 + columnOffset)
                    || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break4 + columnOffset))
                {
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        dgvBreakCurrentCell.Value = futureCellValue;

                        if (!futureCellValue.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            if (dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex + 1].Value.ToString().Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex + 1].Value = "00000";
                            }
                        }
                        else
                        {
                            splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break1)._ColumnDefaultValue;

                            String[] allDefaultValues = splitter.Split('|');

                            for (int z = 0; z < allDefaultValues.Length; z++)
                            {
                                dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex + z].Value = allDefaultValues[z];
                            }
                        }
                    }

                }
                if ((dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break1 + 1 + columnOffset)
                   || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break2 + 1 + columnOffset)
                   || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break3 + 1 + columnOffset)
                   || (dgvBreakCurrentCell.ColumnIndex) == (ColumnsPosition.NesA.Break4 + 1 + columnOffset))
                {
                    if (ReturnValueForLineNumberSlot(e, ref cellValue, ref futureCellValue, ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        dgvBreakCurrentCell.Value = futureCellValue;

                        if (!futureCellValue.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            if (dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex - 1].Value.ToString().Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex - 1].Value = "00";
                            }
                        }
                        else
                        {
                            splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Break1)._ColumnDefaultValue;

                            String[] allDefaultValues = splitter.Split('|');

                            for (int z = 0; z < allDefaultValues.Length; z++)
                            {
                                dgvBreak.Rows[dgvBreakCurrentCell.RowIndex].Cells[dgvBreakCurrentCell.ColumnIndex - 1 + z].Value = allDefaultValues[z];
                            }
                        }
                    }
                }
            }
        }

        private void SetFlagValue(KeyEventArgs e, ref string cellValue, string futureCellValueIfDeleted)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Form frmFlag = new FrmFlag();

                ParameterPasser.flagValue = (cellValue ?? "00");

                frmFlag.ShowDialog();

                cellValue = ParameterPasser.flagValue;

                e.Handled = true; // So normal Enter function isn't executed
            }
            else if (e.KeyCode == Keys.Delete)
            {
                cellValue = futureCellValueIfDeleted;
                e.Handled = true; // So normal Enter function isn't executed
            }
        }

        private void HandleKeyDownForMusicSheets(object sender, KeyEventArgs e, bool isNoiseChannel, bool usePianoKeysForNotes)
        {
            int qtyCellsToEdit = 1; // 1 count for one
            DataGridView dgvMusic = sender as DataGridView;

            if (dgvMusic.RowCount == 0) return;

            // If delete, we delete content of all the cells
            if (e.KeyCode == Keys.Delete)
            {
                qtyCellsToEdit = dgvMusic.SelectedCells.Count;
            }

            for (int i = 0; i < qtyCellsToEdit; i++)
            {
                var dgvMusicCurrentCell = dgvMusic.SelectedCells[i];
                string cellValue = "";
                string futureCellValue = "";
                string futureCellValueIfDeleted = "";
                string letterPressed = "";
                string splitter = "";
                string nullValueForInstruction = "";


                MoveLineByIncrementsOfFour(e, dgvMusic);

                if (dgvMusicCurrentCell.Value != null)
                {
                    cellValue = dgvMusicCurrentCell.Value.ToString();
                }


                if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Note + columnOffset)
                {
                    nullValueForInstruction = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue;
                    if (isNoiseChannel)
                    {
                        if (e.KeyCode == Keys.Enter)
                        {
                            futureCellValue = "0-#";
                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Space)
                        {
                            futureCellValue = MusicEngineFixedNotesDatas.Names.NesA.Delay;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Insert)
                        {
                            futureCellValue = MusicEngineFixedNotesDatas.Names.NesA.Skip;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Delete)
                        {
                            futureCellValue = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (dgvMusicCurrentCell.Value.ToString() != nullValueForInstruction)
                        {
                            if (cellValue != MusicEngineFixedNotesDatas.Names.NesA.Delay && cellValue != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                            {
                                futureCellValue = cellValue; // Until it has a value

                                letterPressed = ReturnNoteLetterPressed(e, usePianoKeysForNotes);
                                if (letterPressed != "" && letterPressed != "G")
                                {
                                    futureCellValue = letterPressed + "-#";

                                    e.Handled = true; // So normal Enter function isn't executed
                                }
                                else
                                {
                                    letterPressed = ReturnNumberPressed(e);
                                    if (letterPressed != "")
                                    {
                                        futureCellValue = letterPressed + "-#";

                                        e.Handled = true; // So normal Enter function isn't executed
                                    }
                                }
                            }
                        }
                        // If value has to be changed
                        if (futureCellValue != nullValueForInstruction)
                        {
                            if (futureCellValue != "")
                            {
                                dgvMusicCurrentCell.Value = futureCellValue;
                            }
                        }
                        else
                        {
                            dgvMusicCurrentCell.Value = futureCellValue;
                        }
                    }
                    else
                    {
                        if (e.KeyCode == Keys.Enter)
                        {
                            futureCellValue = "C-0";
                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Space)
                        {
                            futureCellValue = MusicEngineFixedNotesDatas.Names.NesA.Delay;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Insert)
                        {
                            futureCellValue = MusicEngineFixedNotesDatas.Names.NesA.Skip;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (e.KeyCode == Keys.Delete)
                        {
                            futureCellValue = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue;

                            e.Handled = true; // So normal Enter function isn't executed
                        }
                        else if (dgvMusicCurrentCell.Value.ToString() != nullValueForInstruction)
                        {
                            if (cellValue != MusicEngineFixedNotesDatas.Names.NesA.Delay && cellValue != MusicEngineFixedNotesDatas.Names.NesA.Skip)
                            {
                                futureCellValue = cellValue; // Until it has a value

                                letterPressed = ReturnNoteLetterPressed(e, usePianoKeysForNotes);
                                if (letterPressed != "")
                                {
                                    futureCellValue = letterPressed + cellValue.Substring(1);

                                    e.Handled = true; // So normal Enter function isn't executed
                                }
                                else
                                {
                                    letterPressed = ReturnNumberPressed(e);
                                    if (letterPressed != "")
                                    {
                                        futureCellValue = cellValue.Substring(0, 2) + letterPressed;

                                        e.Handled = true; // So normal Enter function isn't executed
                                    }
                                    else
                                    {
                                        if (e.KeyCode == Keys.Oem7 || e.KeyCode == Keys.S)
                                        {
                                            if (cellValue.Substring(1, 1) == "#")
                                            {
                                                futureCellValue = cellValue.Substring(0, 1) + "-" + cellValue.Substring(2, 1);
                                            }
                                            else
                                            {
                                                futureCellValue = cellValue.Substring(0, 1) + "#" + cellValue.Substring(2, 1);
                                            }

                                            e.Handled = true; // So normal Enter function isn't executed
                                        }
                                        else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.M)
                                        {
                                            if (cellValue.Substring(1, 1) == "#")
                                            {
                                                futureCellValue = cellValue.Substring(0, 1) + "-" + cellValue.Substring(2, 1);
                                            }
                                            else
                                            {
                                                futureCellValue = cellValue.Substring(0, 1) + "#" + cellValue.Substring(2, 1);
                                            }

                                            e.Handled = true; // So normal Enter function isn't executed
                                        }
                                    }
                                }
                            }
                        }
                        // If value has to be changed
                        if (futureCellValue != nullValueForInstruction)
                        {
                            if (futureCellValue != "")
                            {
                                if (futureCellValue.Substring(2) == "9")
                                {
                                    // Only few notes with 9 exist
                                    if (futureCellValue == "C-9" || futureCellValue == "C#9" || futureCellValue == "D-9" || futureCellValue == "D#9" || futureCellValue == "E-9" || futureCellValue == "F-9" || futureCellValue == "F#9")
                                    {
                                        dgvMusicCurrentCell.Value = futureCellValue;
                                    }
                                }
                                else
                                {
                                    if (futureCellValue.Substring(0, 2) != "B#" && futureCellValue.Substring(0, 2) != "E#")
                                    {
                                        // Note is sure to be valid
                                        dgvMusicCurrentCell.Value = futureCellValue;
                                    }
                                }
                            }
                        }
                        else
                        {
                            dgvMusicCurrentCell.Value = futureCellValue;
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Volume + columnOffset)
                {
                    string maxVolumeValue = "0F";

                    // Will need to identify max value for snes!!!
                    if (_GameType == GameType.SnesA)
                    {
                        maxVolumeValue = "FF";
                    }

                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Volume)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex(maxVolumeValue), futureCellValueIfDeleted))
                    {
                        if (futureCellValue != futureCellValueIfDeleted)
                        {
                            if (futureCellValue != "")
                            {
                                dgvMusicCurrentCell.Value = futureCellValue;
                            }
                        }
                        else
                        {
                            dgvMusicCurrentCell.Value = futureCellValue;
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Instrument + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Instrument)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex(70), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Octave + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Octave)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("07"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.ToneLenght + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.TunePitch + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TunePitch)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.PitchSlide + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlide)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.SetFlags + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Flags)._ColumnDefaultValue;
                    SetFlagValue(e, ref cellValue, futureCellValueIfDeleted);

                    dgvMusicCurrentCell.Value = cellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.ToneType + columnOffset)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        if (cellValue == GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneType)._ColumnDefaultValue)
                        {
                            dgvMusicCurrentCell.Value = "12.5%";
                        }
                        else if (cellValue == "")
                        {
                            dgvMusicCurrentCell.Value = "12.5%";
                        }
                        else if (cellValue == "12.5%")
                        {
                            dgvMusicCurrentCell.Value = "25.0%";
                        }
                        else if (cellValue == "25.0%")
                        {
                            dgvMusicCurrentCell.Value = "50.0%";
                        }
                        else if (cellValue == "50.0%")
                        {
                            dgvMusicCurrentCell.Value = "75.0%";
                        }
                        else if (cellValue == "75.0%")
                        {
                            dgvMusicCurrentCell.Value = "12.5%";
                        }
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusicCurrentCell.Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneType)._ColumnDefaultValue;
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Transpose + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Transpose)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.GlobalTranspose + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalTranspose)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Speed + columnOffset)
                {
                    futureCellValueIfDeleted = ColumnsEmptyValue.symbolRequiredForEmptyCells;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex + 1, dgvMusicCurrentCell.RowIndex].Value = "00";
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex + 1, dgvMusicCurrentCell.RowIndex].Value = null;

                        splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Speed)._ColumnDefaultValue;

                        String[] allDefaultValues = splitter.Split('|');

                        for (int z = 0; z < allDefaultValues.Length; z++)
                        {
                            dgvMusic[dgvMusicCurrentCell.ColumnIndex + z, dgvMusicCurrentCell.RowIndex].Value = allDefaultValues[z];
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Speed + 1 + columnOffset)
                {
                    futureCellValueIfDeleted = ColumnsEmptyValue.symbolRequiredForEmptyCells;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex - 1, dgvMusicCurrentCell.RowIndex].Value = "00";

                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Speed)._ColumnDefaultValue;

                        String[] allDefaultValues = splitter.Split('|');

                        for (int z = 0; z < allDefaultValues.Length; z++)
                        {
                            dgvMusic[dgvMusicCurrentCell.ColumnIndex - 1 + z, dgvMusicCurrentCell.RowIndex].Value = allDefaultValues[z];
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.OctavePlus + columnOffset)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusicCurrentCell.Value = "TOG";
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusicCurrentCell.Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.OctavePlus)._ColumnDefaultValue;
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Connect + columnOffset)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusicCurrentCell.Value = "TOG";
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusicCurrentCell.Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Connect)._ColumnDefaultValue;
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Triplet + columnOffset)
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusicCurrentCell.Value = "TOG";
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusicCurrentCell.Value = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Triplet)._ColumnDefaultValue;
                        e.Handled = true; // So normal Enter function isn't executed
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.SfxToggle + columnOffset)
                {
                    futureCellValueIfDeleted = ColumnsEmptyValue.symbolRequiredForEmptyCells;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex + 1, dgvMusicCurrentCell.RowIndex].Value = "00";
                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex + 1, dgvMusicCurrentCell.RowIndex].Value = null;

                        splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.SfxToggle)._ColumnDefaultValue;

                        String[] allDefaultValues = splitter.Split('|');

                        for (int z = 0; z < allDefaultValues.Length; z++)
                        {
                            dgvMusic[dgvMusicCurrentCell.ColumnIndex + z, dgvMusicCurrentCell.RowIndex].Value = allDefaultValues[z];
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.SfxToggle + 1 + columnOffset)
                {
                    futureCellValueIfDeleted = ColumnsEmptyValue.symbolRequiredForEmptyCells;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                    if (e.KeyCode == Keys.Enter)
                    {
                        dgvMusic[dgvMusicCurrentCell.ColumnIndex - 1, dgvMusicCurrentCell.RowIndex].Value = "00";

                    }
                    else if (e.KeyCode == Keys.Delete)
                    {
                        splitter = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.SfxToggle)._ColumnDefaultValue;

                        String[] allDefaultValues = splitter.Split('|');

                        for (int z = 0; z < allDefaultValues.Length; z++)
                        {
                            dgvMusic[dgvMusicCurrentCell.ColumnIndex - 1 + z, dgvMusicCurrentCell.RowIndex].Value = allDefaultValues[z];
                        }
                    }
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Panning + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Panning)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.GlobalVolume + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.GlobalVolume)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.VibratoDepth + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoDepth)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.TremoloVolume + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.TremoloVolume)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.VibratoOrTremolFrequency + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.VibratoOrTremolFrequency)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.ins1A03 + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ins1A03)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.PitchSlideUnk + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PitchSlideUnk)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Sfx + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Sfx)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.ToneLenght2 + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.ToneLenght2)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.PannMovementSpeed + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.PannMovementSpeed)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
                else if ((dgvMusicCurrentCell.ColumnIndex) == ColumnsPosition.NesA.Unknown1F + columnOffset)
                {
                    futureCellValueIfDeleted = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Unknown1F)._ColumnDefaultValue;
                    if (ReturnValueForNumericSlot(e, ref cellValue, ref futureCellValue, new Hex("FF"), futureCellValueIfDeleted))
                        dgvMusicCurrentCell.Value = futureCellValue;
                }
            }
        }
        
        private void DgvMusicNoise_KeyDown(object sender, KeyEventArgs e)
        {
            if (GetGameType() == GameType.NesA)
            {
                HandleKeyDownForMusicSheets(sender, e, true, _UsePianoKeysForNotes);
            }
            else
            {
                HandleKeyDownForMusicSheets(sender, e, false, _UsePianoKeysForNotes);
            }
        }



        private void DgvMusic_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDownForMusicSheets(sender, e, false, _UsePianoKeysForNotes);
        }


        /// <summary>
        /// Add one data grid view to a list.
        /// </summary>
        /// <param name="listDataGrid">ref (may be Music, Loop, Break</param>
        /// <param name="name"></param>
        private void AddOneDataGridToList(ref List<DataGridView> listDataGrid, int channelIndex, PageName pageName, GameType gameType)
        {
            DataGridView temp = new DataGridView();

            // Column count
            int columnCount = 0;
            string name = "";
            string headerText = "";
            string columnWidthText = "";
            
            // At first, the idea was to create/delete column depending on how many where need with console.
            // Now we create them all
            int musicGridColumnCount = 29;
            int loopGridColumnCount = 9;
            int breakGridColumnCount = 8;
            
            if (pageName == PageName.Notes)
            {
                name = "dgvMusic";
                columnCount = musicGridColumnCount;
            }
            else if (pageName == PageName.Loops)
            {
                name = "dgvLoop";
                columnCount = loopGridColumnCount;
            }
            else
            {
                name = "dgvBreak";
                columnCount = breakGridColumnCount;
            }

            name += channelIndex.ToString();

            // Parameters settings
            // Misc properties
            temp.CausesValidation = false;
            temp.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            temp.EditMode = DataGridViewEditMode.EditProgrammatically;      // Cells won't be editable before call of BeginEdit
                                                                            // Lock users actions
            temp.AllowUserToAddRows = false;
            temp.AllowUserToDeleteRows = false;
            temp.AllowUserToResizeColumns = false;
            temp.AllowUserToResizeRows = false;
            temp.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            // Show properties
            temp.ShowCellErrors = false;
            temp.ShowCellToolTips = false;
            temp.ShowEditingIcon = false;
            temp.ShowRowErrors = false;
            // Set Invisible headers
            temp.RowHeadersVisible = false;
            temp.ColumnHeadersVisible = true;
            // Dimensions
            temp.Width = 1150;
            temp.Height = 450;
            // Position
            temp.Top = 180;
            temp.Left = 11;
            // Name
            temp.Name = name;
            // Quantity of columns
            temp.ColumnCount = columnCount + columnOffset;
            // Background color
            temp.BackgroundColor = Color.Gray;
            temp.DefaultCellStyle.BackColor = Color.Black;
            temp.DefaultCellStyle.ForeColor = Color.FromArgb(0, 255, 0);
            temp.CellBorderStyle = DataGridViewCellBorderStyle.None;
            temp.Font = new Font("Courier New", 13, FontStyle.Bold);
            temp.ColumnHeadersDefaultCellStyle.Font = new Font("Courier New", 10, FontStyle.Regular);
            temp.Columns[0].DefaultCellStyle.Font = new Font("Courier New", 10, FontStyle.Regular);
            temp.RowTemplate.Height = 16; // Height of cells
            // The first column with line number width
            temp.Columns[0].Width = 40;
            for (int i = 0; i < temp.ColumnCount; i++)
            {
                temp.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                if (i >= columnOffset)
                {
                    headerText = "";

                    for (int y = 0; y < GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA.Count; y++)
                    {
                        if (GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[y]._ColumnPosition == (i - columnOffset))
                        {
                            if (GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[y]._Page == pageName)
                            {
                                headerText = GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[y]._ColumnTitle;
                                columnWidthText = GetMusicEngineInstructionsDatas.fixedMusicEngineDataArrayNesA[y]._ColumnWidth;

                                String[] allHeaders = headerText.Split('|');
                                String[] allColumnWidth = columnWidthText.Split('|');

                                for (int z = 0; z < allHeaders.Length; z++)
                                {
                                    temp.Columns[i + z].HeaderText = allHeaders[z];
                                }
                                for (int z = 0; z < allColumnWidth.Length; z++)
                                {
                                    temp.Columns[i + z].Width = Convert.ToInt32(allColumnWidth[z]);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (gameType == GameType.NesA)
            {
                if (pageName == PageName.Notes)
                {
                    if (channelIndex == 3)
                    {
                        temp.KeyDown += new KeyEventHandler(DgvMusicNoise_KeyDown);
                    }
                    else
                    {
                        temp.KeyDown += new KeyEventHandler(DgvMusic_KeyDown);
                    }
                    temp.MouseClick += new MouseEventHandler(DgvMusicLoopBreak_MouseClick);
                }
                else if (pageName == PageName.Loops)
                {
                    temp.KeyDown += new KeyEventHandler(DgvLoop_KeyDown);
                    temp.MouseClick += new MouseEventHandler(DgvMusicLoopBreak_MouseClick);
                }
                else
                {
                    temp.KeyDown += new KeyEventHandler(DgvBreak_KeyDown);
                    temp.MouseClick += new MouseEventHandler(DgvMusicLoopBreak_MouseClick);
                }
            }

            // Nes columns name

            // Add form to list and form
            listDataGrid.Add(temp);
            _currentForm.Controls.Add(temp);
        }


        /// <summary>
        /// Hide all sheets
        /// </summary>
        private void HideAllSheet()
        {
            for (int i = 0; i < 8; i++)
            {
                listMusicDataGridView[i].Visible = false;
                listLoopDataGridView[i].Visible = false;
                listBreakDataGridView[i].Visible = false;
            }
        }
        
        /// <summary>
        /// Show one specific sheet (0 = Music, 1 = Loop, 2 = Break)
        /// </summary>
        /// <param name="channelSelected"></param>
        /// <param name="sheetSelected"></param>
        private void ShowOneSheet(int channelSelected, RdButtonSheetType sheetSelected)
        {
            HideAllSheet();

            // Always remember last selected
            _lastChannelSelected = channelSelected;
            _lastSheetSelected = sheetSelected;

            if (sheetSelected == RdButtonSheetType.Music) listMusicDataGridView[channelSelected].Visible = true;
            if (sheetSelected == RdButtonSheetType.Loop) listLoopDataGridView[channelSelected].Visible = true;
            if (sheetSelected == RdButtonSheetType.Break) listBreakDataGridView[channelSelected].Visible = true;
        }

        /// <summary>
        /// Show one sheet for a given channel (sheet type unchanged)
        /// </summary>
        /// <param name="channelSelected"></param>
        public void ShowOneSheet(int channelSelected)
        {
            ShowOneSheet(channelSelected, _lastSheetSelected);
        }

        /// <summary>
        /// Show one sheet for a given sheet (channel index unchanged)
        /// </summary>
        /// <param name="channelSelected"></param>
        public void ShowOneSheet(RdButtonSheetType sheetSelected)
        {
            ShowOneSheet(_lastChannelSelected, sheetSelected);
        }

        /// <summary>
        /// Content of a channel is written to appropriate DataGridView
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="channelToWrite"></param>
        /// <param name="tripletMode">True if there is a triplet in the song read</param>
        public void WriteChannelInDataGridView(int channel, ChannelInterpreterFromHex channelToWrite)
        {
            int i = 0;
            List<MusicLine> _channelLines = channelToWrite.GetChannelLines();

            // Create needed number of lines in DataGridView, but before clear them
            listMusicDataGridView[channel].RowCount = 0;
            listLoopDataGridView[channel].RowCount = 0;
            listBreakDataGridView[channel].RowCount = 0;
            listMusicDataGridView[channel].RowCount = _channelLines.Count;
            listLoopDataGridView[channel].RowCount = _channelLines.Count;
            listBreakDataGridView[channel].RowCount = _channelLines.Count;

            try
            {
                for (i = 0; i < _channelLines.Count; i++)
                {
                    WriteLineInDataGridView(i, channel, _channelLines[i]);
                }

                ColorLineOfDataGridViews(channel);
            }
            catch (Exception)
            {
                // No bug should happen with a valid song
                throw;
            }
        }

        /// <summary>
        /// Transfer (and validate) channel from DataGridView to variables
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="tripletMode"></param>
        public void ReadChannelInDataGridView(out List<List<MusicLine>> _channelLinesInMemory, GameType gameType)
        {
            string currentInstructionName = "";
            string currentValueRead = "";
            string currentNote = "";
            string tempStr = "";
            int qtyChannel = 4;
            int qtyLine = 0;
            int currentOctave = 0;
            int tempInt = 0;

            if (gameType == GameType.SnesA)
            {
                qtyChannel = 8;
            }

            _channelLinesInMemory = new List<List<MusicLine>>();
            
            for (int currentChannel = 0; currentChannel < qtyChannel; currentChannel++)
            {
                _channelLinesInMemory.Add(new List<MusicLine>());
                currentOctave = 0;

                qtyLine = listMusicDataGridView[currentChannel].RowCount;

                for (int i = 0; i < qtyLine; i++)
                {
                    _channelLinesInMemory[currentChannel].Add(new MusicLine());

                    for (int y = 0 + columnOffset; y < listMusicDataGridView[currentChannel].ColumnCount; y++)
                    {
                        if (!listMusicDataGridView[currentChannel].Rows[i].Cells[y].Value.ToString().Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                        {
                            currentValueRead = listMusicDataGridView[currentChannel].Rows[i].Cells[y].Value.ToString();
                            if (!currentValueRead.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                currentInstructionName = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(y - columnOffset, PageName.Notes)._Name;

                                // Note instruction is done at the end separately
                                if (currentInstructionName != MusicEngineFixedInstructionsDatas.Names.Note)
                                {
                                    if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Volume)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetVolume(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Instrument)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetInstrument(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Octave)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetOctave(currentValueRead);

                                        // Set octave for notes
                                        currentOctave = (new Hex(currentValueRead)).GetValueAsInt();
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.ToneLenght)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetToneLenght(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.TunePitch)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetTunePitch(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.PitchSlide)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetPitchSlide(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Flags)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetFlags(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.ToneType && gameType == GameType.NesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetToneType(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Transpose)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetTranspose(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.GlobalTranspose)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetGlobalTranspose(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Speed)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetSpeed(currentValueRead, listMusicDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString());

                                        // Skip second speed column since it was just read
                                        y++;
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Connect)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetConnect(true);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Triplet)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetTriplet();
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.OctavePlus)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetOctavePlus();
                                    }





                                    // Snes only commands
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.SfxToggle && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetSfxToggle(currentValueRead, listMusicDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString());

                                        // Skip second speed column since it was just read
                                        y++;
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Panning && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetPanning(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.GlobalVolume && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetGlobalVolume(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.VibratoDepth && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetVibratoDepth(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.TremoloVolume && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetTremoloVolume(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.VibratoOrTremolFrequency && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetVibratoOrTremolFrequency(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.ins1A03 && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].Set1A03(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.PitchSlideUnk && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetPitchSlideUnk(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Sfx && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetSfx(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.ToneLenght2 && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetToggleFullNoteLenght(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.PannMovementSpeed && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetPanningMovementSpeed(currentValueRead);
                                    }
                                    else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Unknown1F && gameType == GameType.SnesA)
                                    {
                                        _channelLinesInMemory[currentChannel][i].SetUnknown1F(currentValueRead);
                                    }
                                }
                            }
                        }
                    }

                    // We read note now that instructions are done on the line
                    currentValueRead = GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue;

                    if (listMusicDataGridView[currentChannel].Rows[i].Cells[ColumnsPosition.NesA.Note + columnOffset].Value.ToString() != GetMusicEngineInstructionsDatas.GetDatasByInstructionName(MusicEngineFixedInstructionsDatas.Names.Note)._ColumnDefaultValue)
                    {
                        currentValueRead = listMusicDataGridView[currentChannel].Rows[i].Cells[ColumnsPosition.NesA.Note + columnOffset].Value.ToString();
                    }

                    if (!currentValueRead.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                    {
                        if (currentChannel == 3 && gameType == GameType.NesA)
                        {
                            currentInstructionName = MusicEngineFixedInstructionsDatas.Names.Note;
                            currentNote = currentValueRead;
                            
                        }
                        else
                        {
                            currentInstructionName = MusicEngineFixedInstructionsDatas.Names.Note;

                            if (ReturnNoteInZeroToTwoValue(currentOctave, ref currentValueRead, out currentNote) == false)
                            {
                                throw new KnownException("Note on channel: " + ChannelName.ReturnChannelNameByChannelIndex(currentChannel, gameType) + ", Line: " + i.ToString() + " contains a note that cannot be played by current octave.");
                            }
                        }
                        _channelLinesInMemory[currentChannel][i].SetNote(currentNote);
                    }
                }
                
                for (int i = 1; i < qtyLine; i++)
                {
                    for (int y = 0 + columnOffset; y < listLoopDataGridView[currentChannel].ColumnCount; y++)
                    {
                        if (listLoopDataGridView[currentChannel].Rows[i].Cells[y].Value != null)
                        {
                            currentValueRead = listLoopDataGridView[currentChannel].Rows[i].Cells[y].Value.ToString();
                            if (!currentValueRead.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                currentInstructionName = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(y - columnOffset, PageName.Loops)._Name;
                                
                                if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Loop1)
                                {
                                    tempStr = listLoopDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetLoop1(Int32.Parse(currentValueRead), tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Loop on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Loop2)
                                {
                                    tempStr = listLoopDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetLoop2(Int32.Parse(currentValueRead), tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Loop on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Loop3)
                                {
                                    tempStr = listLoopDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetLoop3(Int32.Parse(currentValueRead), tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Loop on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Loop4)
                                {
                                    tempStr = listLoopDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetLoop4(Int32.Parse(currentValueRead), tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Loop on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Jump)
                                {
                                    tempInt = Int32.Parse(currentValueRead);
                                    _channelLinesInMemory[currentChannel][i].SetJump(tempInt);

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Jump on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < qtyLine; i++)
                {
                    for (int y = 0 + columnOffset; y < listBreakDataGridView[currentChannel].ColumnCount; y++)
                    {
                        if (listBreakDataGridView[currentChannel].Rows[i].Cells[y].Value != null)
                        {
                            currentValueRead = listBreakDataGridView[currentChannel].Rows[i].Cells[y].Value.ToString();
                            if (!currentValueRead.Contains(ColumnsEmptyValue.symbolRequiredForEmptyCells))
                            {
                                currentInstructionName = GetMusicEngineInstructionsDatas.GetDatasByColumnPositionAndPage(y - columnOffset, PageName.Breaks)._Name;

                                if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Break1)
                                {
                                    tempStr = listBreakDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetBreak1(currentValueRead, tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Break on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Break2)
                                {
                                    tempStr = listBreakDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetBreak2(currentValueRead, tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Break on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Break3)
                                {
                                    tempStr = listBreakDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetBreak3(currentValueRead, tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Break on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                                else if (currentInstructionName == MusicEngineFixedInstructionsDatas.Names.Break4)
                                {
                                    tempStr = listBreakDataGridView[currentChannel].Rows[i].Cells[y + 1].Value.ToString();
                                    tempInt = Int32.Parse(tempStr);

                                    _channelLinesInMemory[currentChannel][i].SetBreak4(currentValueRead, tempInt, gameType);

                                    // Skip second column since it was just read
                                    y++;

                                    // If line we are jumping to is out of bound
                                    if (tempInt > qtyLine)
                                    {
                                        throw new KnownException("Break on channel: " + (currentChannel + i).ToString() + ", Line: " + i.ToString() + " contains a line number exceeding line quantities.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}