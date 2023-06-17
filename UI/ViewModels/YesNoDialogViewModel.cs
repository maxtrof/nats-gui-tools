using ReactiveUI;

namespace UI.ViewModels;

internal sealed class YesNoDialogViewModel : ViewModelBase
{
    private DialogResult Result { get; } = new()
    {
        Result = DialogResultEnum.None
    };

    public ReactiveCommand<DialogResultEnum, DialogResult> SetResult { get; }

    /// <summary> Dialog title </summary>
    public string Title { get; set; } = "User prompt";

    /// <summary> Dialog text </summary>
    public string? Text { get; set; }

    /// <summary>
    /// ctor
    /// </summary>
    public YesNoDialogViewModel()
    {
        SetResult = ReactiveCommand.Create<DialogResultEnum, DialogResult>(result =>
        {
            Result.Result = result;
            return Result;
        });
    }
}

public class DialogResult
{
    public DialogResultEnum Result { get; set; }
}

public enum DialogResultEnum
{
    None = 0,
    Yes = 1,
    No = 2,
    Ok = 3,
    Cancel = 4
}