﻿namespace AvaloniaThemeManager.Theme;

/// <summary>
/// 
/// </summary>
public interface ISkinManager
{
    /// <summary>
    /// 
    /// </summary>
    Skin? CurrentSkin { get; }
    /// <summary>
    /// 
    /// </summary>
    event EventHandler? SkinChanged;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="skin"></param>
    void RegisterSkin(string? name, Skin? skin);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Skin? GetSkin(string? name);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    List<string> GetAvailableSkinNames();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="skinName"></param>
    void ApplySkin(string? skinName);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="skin"></param>
    void ApplySkin(Skin? skin);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="themeName"></param>
    void SaveSelectedTheme(string? themeName);
    /// <summary>
    /// 
    /// </summary>
    void LoadSavedTheme();
}