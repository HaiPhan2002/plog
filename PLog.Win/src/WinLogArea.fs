namespace PLog

open Eto.Forms
open System.Drawing
open FastColoredTextBoxNS
open System
open System.Windows.Forms

type Mode =
    { BackColor: Color
      SelColor: Color
      ErrorStyle: TextStyle
      WarningStyle: TextStyle
      DebugStyle: TextStyle
      InfoStyle: TextStyle }

type MyFCTB() =
    inherit FastColoredTextBox()

    let mutable lastWheelTime = DateTime.MinValue

    override this.OnMouseWheel(e: MouseEventArgs) =
        if (Control.ModifierKeys &&& Keys.Shift) = Keys.Shift then
            // Tính khoảng thời gian giữa 2 lần wheel
            let now = DateTime.Now
            let dt = (now - lastWheelTime).TotalMilliseconds
            lastWheelTime <- now

            // Tính gia tốc dựa trên dt
            let accel =
                if dt < 100.0 then 3   // cuộn nhanh -> 3x
                elif dt < 200.0 then 2  // cuộn vừa -> 2x
                else 1                  // cuộn chậm -> 1x

            let step = this.Font.Height * accel
            let hs = this.HorizontalScroll
            if hs.Visible then
                let newVal = if e.Delta > 0 then hs.Value - step else hs.Value + step
                hs.Value <- Math.Max(hs.Minimum, Math.Min(hs.Maximum, newVal))
                this.UpdateScrollbars()
                this.Invalidate()
        else
            base.OnMouseWheel(e)

type WinLogArea (isDark) =

    static let darkMode =
        { BackColor = Color.Black
          SelColor = Color.FromArgb(150, 255, 255, 255)
          ErrorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular)
          WarningStyle = new TextStyle(Brushes.Yellow, null, FontStyle.Regular)
          DebugStyle = new TextStyle(Brushes.LightGreen, null, FontStyle.Regular)
          InfoStyle = new TextStyle(Brushes.White, null, FontStyle.Regular) }

    static let lightMode =
        { BackColor = Color.White
          SelColor = Color.FromArgb(150, 0, 0, 255)
          ErrorStyle = new TextStyle(Brushes.Red, null, FontStyle.Regular)
          WarningStyle = new TextStyle(Brushes.Brown, null, FontStyle.Regular)
          DebugStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular)
          InfoStyle = new TextStyle(Brushes.Black, null, FontStyle.Regular) }

    let mutable currentMode = if isDark then darkMode else lightMode

    let fctb = new MyFCTB (ReadOnly = true, BackColor = currentMode.BackColor, ForeColor = Color.White,
                                       SelectionColor = currentMode.SelColor,
                                       Font = new Font("Consolas", 14.f))

    let appendLines lines =
        fctb.BeginUpdate ()
        fctb.Selection.BeginUpdate ()

        let userSelection = fctb.Selection.Clone ()
        let shouldNotGoEnd = (not userSelection.IsEmpty) || (userSelection.Start.iLine < fctb.LinesCount - 1)
        
        fctb.TextSource.CurrentTB <- fctb
        
        for (text, severity) in lines do
            let style =
                match severity with
                | Domain.Err     -> currentMode.ErrorStyle
                | Domain.Warning -> currentMode.WarningStyle
                | Domain.Debug   -> currentMode.DebugStyle
                | Domain.Info    -> currentMode.InfoStyle
            fctb.AppendText (text + Environment.NewLine, style)

        if shouldNotGoEnd then
            fctb.Selection.Start <- userSelection.Start
            fctb.Selection.End <- userSelection.End
        else
            fctb.GoEnd ()
            
        fctb.Selection.EndUpdate ()
        fctb.EndUpdate ()

    let changeMode mode =
        currentMode <- mode
        fctb.BackColor <- currentMode.BackColor
        fctb.SelectionColor <- currentMode.SelColor
    
    interface LogArea with

        member this.GetEtoControl () =
            fctb.ToEto ()

        member this.Clear () =
            fctb.Clear ()

        member this.GoEnd () =
            fctb.GoEnd ()

        member this.SetWrap value =
            fctb.WordWrap <- value

        member this.AppendLines lines =
            appendLines lines

        member this.ChangeMode isDark =
            if isDark && currentMode <> darkMode then
                changeMode darkMode
            elif not isDark && currentMode <> lightMode then
                changeMode lightMode
