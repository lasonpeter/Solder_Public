using System;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Solder.Controls.TemplatedControls;

public class InstanceTemplatedControl : TemplatedControl
{
    #region Name

    public new static readonly StyledProperty<string> NameProperty =
        AvaloniaProperty.Register<InstanceTemplatedControl, string>(nameof(Name));

    public new string Name
    {
        get => GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    #endregion

    #region Content

    public static readonly StyledProperty<string> VersionProperty =
        AvaloniaProperty.Register<InstanceTemplatedControl, string>(nameof(Version));

    public string Version
    {
        get => GetValue(VersionProperty);
        set => SetValue(VersionProperty, value);
    }

    #endregion

    #region Id

    public static readonly StyledProperty<Guid> IdProperty =
        AvaloniaProperty.Register<InstanceTemplatedControl, Guid>(nameof(Id));

    public Guid Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }

    #endregion


    #region IsAddingInstanceCard

    public static readonly StyledProperty<bool> IsAddInstanceCardProperty =
        AvaloniaProperty.Register<InstanceTemplatedControl, bool>(nameof(IsAddInstanceCard));

    public bool IsAddInstanceCard
    {
        get => GetValue(IsAddInstanceCardProperty);
        set => SetValue(IsAddInstanceCardProperty, value);
    }

    #endregion
}