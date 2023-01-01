using System.Windows;
using System.Windows.Controls;

namespace FixedFileToSqlServerTool.Views;

public class PasswordBoxHelper : DependencyObject
{
    public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
        "IsAttached",
        typeof(bool),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(false, PasswordBoxHelper.IsAttachedProperty_Changed));

    public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
        "Password",
        typeof(string),
        typeof(PasswordBoxHelper),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PasswordBoxHelper.PasswordProperty_Changed));

    public static bool GetIsAttached(DependencyObject dp) =>
        (bool)dp.GetValue(PasswordBoxHelper.IsAttachedProperty);

    public static string GetPassword(DependencyObject dp) =>
        (string)dp.GetValue(PasswordBoxHelper.PasswordProperty);

    public static void SetIsAttached(DependencyObject dp, bool value) =>
        dp.SetValue(PasswordBoxHelper.IsAttachedProperty, value);

    public static void SetPassword(DependencyObject dp, string value) =>
        dp.SetValue(PasswordBoxHelper.PasswordProperty, value);

    private static void IsAttachedProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;

        if ((bool)e.OldValue)
        {
            passwordBox.PasswordChanged -= PasswordBoxHelper.PasswordBox_PasswordChanged;
        }

        if ((bool)e.NewValue)
        {
            passwordBox.PasswordChanged += PasswordBoxHelper.PasswordBox_PasswordChanged;
        }
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        PasswordBoxHelper.SetPassword(passwordBox, passwordBox.Password);
    }

    private static void PasswordProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var passwordBox = sender as PasswordBox;
        var newPassword = (string)e.NewValue;

        if (!GetIsAttached(passwordBox))
        {
            SetIsAttached(passwordBox, true);
        }

        if ((string.IsNullOrEmpty(passwordBox.Password) && string.IsNullOrEmpty(newPassword)) ||
            passwordBox.Password == newPassword)
        {
            return;
        }

        passwordBox.PasswordChanged -= PasswordBoxHelper.PasswordBox_PasswordChanged;
        passwordBox.Password = newPassword;
        passwordBox.PasswordChanged += PasswordBoxHelper.PasswordBox_PasswordChanged;
    }
}
