using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotBase.Base;
using TelegramBotBase.Enums;
using TelegramBotBase.Form;
using TelegramBotBase.Tools;

namespace PersonalBot.Views.Components
{
    public class SafeCalendarPicker : ControlBase
    {
        private static Localization Local { get; } = new Localization();
        public DateTime SelectedDate { get; set; }
        public DateTime VisibleMonth { get; set; }
        public DayOfWeek FirstDayOfWeek { get; set; }
        public CultureInfo Culture { get; set; }
        public string Title { get; set; } = Local["CalendarPicker_Title"];
        public eMonthPickerMode PickerMode { get; set; }
        public bool EnableDayView { get; set; } = true;
        public bool EnableMonthView { get; set; } = true;
        public bool EnableYearView { get; set; } = true;
        private int? MessageId { get; set; }

        public SafeCalendarPicker()
        {
            SelectedDate = DateTime.Today;
            VisibleMonth = DateTime.Today;
            FirstDayOfWeek = DayOfWeek.Monday;
            Culture = new CultureInfo("ru-ru");
            PickerMode = eMonthPickerMode.day;
        }
        
        private class Localization
        {
            public Dictionary<string, string> Values = new();

            public string this[string key] => Values[key];

            public Localization()
            {
                Values["Language"] = "Russian";
                Values["ButtonGrid_Title"] = "Меню";
                Values["ButtonGrid_NoItems"] = "Пусто.";
                Values["ButtonGrid_PreviousPage"] = "◀️";
                Values["ButtonGrid_NextPage"] = "▶️";
                Values["ButtonGrid_CurrentPage"] = "Страница {0} из {1}";
                Values["ButtonGrid_SearchFeature"] = "💡 Send a message to filter the list. Click the 🔍 to reset the filter.";
                Values["ButtonGrid_Back"] = "Назад";
                Values["CalendarPicker_Title"] = "Выбрать дату";
                Values["CalendarPicker_PreviousPage"] = "◀️";
                Values["CalendarPicker_NextPage"] = "▶️";
                Values["TreeView_Title"] = "Select node";
                Values["TreeView_LevelUp"] = "🔼 выше";
                Values["ToggleButton_On"] = "Включить";
                Values["ToggleButton_Off"] = "Выключить";
                Values["ToggleButton_OnIcon"] = "⚫";
                Values["ToggleButton_OffIcon"] = "⚪";
                Values["ToggleButton_Title"] = "Переключить";
                Values["PromptDialog_Back"] = "Назад";
                Values["ToggleButton_Changed"] = "Настройки изменены";
            
            }

        }

        public override async Task Action(MessageResult result, string value = null)
        {
            await result.ConfirmAction();

            switch (result.RawData)
            {
                case "$next$":
                    VisibleMonth = PickerMode switch
                    {
                        eMonthPickerMode.day => VisibleMonth.AddMonths(1),
                        eMonthPickerMode.month => VisibleMonth.AddYears(1),
                        eMonthPickerMode.year => VisibleMonth.AddYears(10),
                        _ => VisibleMonth
                    };
                    break;
                case "$prev$":
                    VisibleMonth = PickerMode switch
                    {
                        eMonthPickerMode.day => VisibleMonth.AddMonths(-1),
                        eMonthPickerMode.month => VisibleMonth.AddYears(-1),
                        eMonthPickerMode.year => VisibleMonth.AddYears(-10),
                        _ => VisibleMonth
                    };
                    break;
                case "$monthtitle$":
                    if (EnableMonthView)
                        PickerMode = eMonthPickerMode.month;
                    break;
                case "$yeartitle$":
                    if (EnableYearView)
                        PickerMode = eMonthPickerMode.year;
                    break;
                case "$yearstitle$":
                    if (EnableMonthView)
                        PickerMode = eMonthPickerMode.month;

                    VisibleMonth = SelectedDate;
                    break;
                default:
                    if (result.RawData.StartsWith("d-") && int.TryParse(result.RawData.Split('-')[1], out var day))
                        SelectedDate = new DateTime(VisibleMonth.Year, VisibleMonth.Month, day);

                    if (result.RawData.StartsWith("m-") && int.TryParse(result.RawData.Split('-')[1], out var month) && month is > 0 and < 13)
                    {
                        SelectedDate = new DateTime(VisibleMonth.Year, month, 1);
                        VisibleMonth = SelectedDate;

                        if (EnableDayView)
                            PickerMode = eMonthPickerMode.day;
                    }

                    if (result.RawData.StartsWith("y-") && int.TryParse(result.RawData.Split('-')[1], out var year))
                    {
                        SelectedDate = new DateTime(year, SelectedDate.Month, SelectedDate.Day);
                        VisibleMonth = SelectedDate;

                        if (EnableMonthView)
                            PickerMode = eMonthPickerMode.month;
                    }
                    break;
            }
        }



        public override async Task Render(MessageResult result)
        {
            ButtonForm bf = new ButtonForm();

            switch (PickerMode)
            {
                case eMonthPickerMode.day:

                    var month = VisibleMonth;

                    string[] dayNamesNormal = Culture.DateTimeFormat.ShortestDayNames;
                    string[] dayNamesShifted = Shift(dayNamesNormal, (int)FirstDayOfWeek);

                    bf.AddButtonRow(new ButtonBase(Local["CalendarPicker_PreviousPage"], "$prev$"),
                        new ButtonBase(Culture.DateTimeFormat.MonthNames[month.Month - 1] + " " + month.Year.ToString(),
                            "$monthtitle$"),
                        new ButtonBase(Local["CalendarPicker_NextPage"], "$next$"));

                    bf.AddButtonRow(dayNamesShifted.Select(a => new ButtonBase(a, a)).ToList());

                    //First Day of month
                    var firstDay = new DateTime(month.Year, month.Month, 1);

                    //Last Day of month
                    var lastDay = firstDay.LastDayOfMonth();

                    //Start of Week where first day of month is (left border)
                    var start = firstDay.StartOfWeek(FirstDayOfWeek);

                    //End of week where last day of month is (right border)
                    var end = lastDay.EndOfWeek(FirstDayOfWeek);

                    for (int i = 0; i <= ((end - start).Days / 7); i++)
                    {
                        var lst = new List<ButtonBase>();
                        for (int id = 0; id < 7; id++)
                        {
                            var d = start.AddDays((i * 7) + id);
                            if (d < firstDay | d > lastDay)
                            {
                                lst.Add(new ButtonBase("-", "m-" + d.Day.ToString()));
                                continue;
                            }

                            var day = d.Day.ToString();

                            if (d == DateTime.Today)
                            {
                                day = "(" + day + ")";
                            }

                            lst.Add(new ButtonBase((SelectedDate == d ? "[" + day + "]" : day), "d-" + d.Day.ToString()));
                        }
                        bf.AddButtonRow(lst);
                    }

                    break;

                case eMonthPickerMode.month:

                    bf.AddButtonRow(new ButtonBase(Local["CalendarPicker_PreviousPage"], "$prev$"), new ButtonBase(VisibleMonth.Year.ToString("0000"), "$yeartitle$"), new ButtonBase(Local["CalendarPicker_NextPage"], "$next$"));

                    var months = Culture.DateTimeFormat.MonthNames;

                    var buttons = months.Select((a, b) => new ButtonBase((b == SelectedDate.Month - 1 && SelectedDate.Year == VisibleMonth.Year ? "[ " + a + " ]" : a), "m-" + (b + 1).ToString()));

                    bf.AddSplitted(buttons, 2);

                    break;

                case eMonthPickerMode.year:

                    bf.AddButtonRow(new ButtonBase(Local["CalendarPicker_PreviousPage"], "$prev$"), new ButtonBase("Year", "$yearstitle$"), new ButtonBase(Local["CalendarPicker_NextPage"], "$next$"));

                    var starti = Math.Floor(VisibleMonth.Year / 10f) * 10;

                    for (int i = 0; i < 10; i++)
                    {
                        var m = starti + (i * 2);
                        bf.AddButtonRow(new ButtonBase((SelectedDate.Year == m ? "[ " + m.ToString() + " ]" : m.ToString()), "y-" + m.ToString()), new ButtonBase((SelectedDate.Year == (m + 1) ? "[ " + (m + 1).ToString() + " ]" : (m + 1).ToString()), "y-" + (m + 1).ToString()));
                    }

                    break;

            }


            if (MessageId != null)
            {
                var m = await Device.Edit(MessageId.Value, Title, bf);
            }
            else
            {
                var m = await Device.Send(Title, bf);
                MessageId = m.MessageId;
            }
        }



        public override async Task Cleanup()
        {
            if (MessageId != null)
                await Device.DeleteMessage(MessageId.Value);
        }

        private static T[] Shift<T>(T[] array, int positions)
        {
            var copy = new T[array.Length];
            Array.Copy(array, 0, copy, array.Length - positions, positions);
            Array.Copy(array, positions, copy, 0, array.Length - positions);
            return copy;
        }
    }
}