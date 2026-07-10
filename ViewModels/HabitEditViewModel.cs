using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabitTracker.Models;
using HabitTracker.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace HabitTracker.ViewModels;

[QueryProperty(nameof(HabitId), "id")]
public partial class HabitEditViewModel : BaseViewModel
{
    private readonly IHabitService _habitService;
    private Habit? _habit;

    private static readonly HttpClient _http = new();
    private static readonly JsonSerializerOptions _jsonOpts = new() { PropertyNameCaseInsensitive = true };
    // WARNING: rotate this key — it must not be committed to version control.
    private const string UnsplashKey = "MrB-eBroSZdXSrV-4knpkcHHwQ1_z7JDfF961A35fBo";

    [ObservableProperty] private string _habitId = string.Empty;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string? _icon;
    [ObservableProperty] private string? _backgroundColor;
    [ObservableProperty] private string? _backgroundImageUrl;
    [ObservableProperty] private string? _accentColor;

    [ObservableProperty] private bool _isNameExpanded = true;
    [ObservableProperty] private bool _isIconExpanded;
    [ObservableProperty] private bool _isBackgroundExpanded;
    [ObservableProperty] private bool _isAccentExpanded;

    [ObservableProperty] private string _emojiSearch = string.Empty;
    [ObservableProperty] private string _backgroundSearch = string.Empty;
    [ObservableProperty] private bool _hasBackgroundPhotos;

    public ObservableCollection<string> FilteredEmojis { get; } = new(EmojiOptions);
    public ObservableCollection<UnsplashPhoto> BackgroundPhotos { get; } = new();

    partial void OnEmojiSearchChanged(string value)
    {
        FilteredEmojis.Clear();
        var query = value.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(query))
        {
            foreach (var e in EmojiOptions) FilteredEmojis.Add(e);
            return;
        }
        foreach (var e in EmojiOptions)
        {
            if (_emojiKeywords.TryGetValue(e, out var kw) && kw.Contains(query, StringComparison.OrdinalIgnoreCase))
                FilteredEmojis.Add(e);
        }
    }

    [RelayCommand]
    private void ToggleSection(string section)
    {
        switch (section)
        {
            case "name":       IsNameExpanded       = !IsNameExpanded;       break;
            case "icon":       IsIconExpanded       = !IsIconExpanded;       break;
            case "background": IsBackgroundExpanded = !IsBackgroundExpanded; break;
            case "accent":     IsAccentExpanded     = !IsAccentExpanded;     break;
        }
    }

    private static readonly Dictionary<string, string> _emojiKeywords = new()
    {
        // Fitness & Sport
        {"🏃","run running jog sprint exercise fitness sport"},
        {"🚶","walk walking steps exercise"},
        {"💪","muscle strength workout gym exercise"},
        {"🧘","yoga meditation mindfulness relax"},
        {"🏋️","gym weights lift exercise strength"},
        {"🤸","gymnastics flex stretch exercise"},
        {"🚴","bike cycling exercise cardio"},
        {"🏊","swim swimming exercise water"},
        {"🧗","climb rock climbing exercise"},
        {"⛹️","basketball sport exercise"},
        {"🤾","handball sport exercise"},
        {"🏇","horse riding equestrian sport"},
        {"🤽","water polo swim sport"},
        {"🏂","snowboard snow winter sport"},
        {"⛷️","ski skiing snow winter sport"},
        {"🤺","fencing sword sport"},
        {"🥊","boxing fight sport"},
        {"🎽","sports jersey exercise"},
        {"🥋","martial arts karate judo taekwondo"},
        {"🏄","surf surfing water sport"},
        {"🧜","mermaid swim water"},
        {"🪂","skydive parachute jump"},
        {"🤿","dive scuba underwater water"},
        {"🎿","ski skiing snow winter"},
        {"🛹","skateboard skate sport"},
        {"🏌️","golf sport outdoor"},
        {"🏹","archery bow arrow sport"},
        {"🎣","fishing outdoor hobby"},
        {"🤼","wrestling fight sport"},
        {"🚣","row rowing water sport"},
        // Food & Drink
        {"🍎","apple fruit eat healthy food"},
        {"🥗","salad vegetable healthy food"},
        {"🥦","broccoli vegetable healthy food"},
        {"🍵","tea drink hot relax"},
        {"☕","coffee drink hot morning"},
        {"💧","water drink hydrate"},
        {"🥤","drink juice beverage"},
        {"🍇","grape fruit eat healthy"},
        {"🍓","strawberry fruit eat healthy"},
        {"🥑","avocado fruit eat healthy food"},
        {"🍌","banana fruit eat healthy food"},
        {"🍊","orange fruit eat healthy food"},
        {"🥕","carrot vegetable eat healthy food"},
        {"🥚","egg protein food breakfast"},
        {"🫐","blueberry fruit eat healthy"},
        {"🥝","kiwi fruit eat healthy"},
        {"🍋","lemon fruit eat healthy"},
        {"🥜","peanut nut protein food"},
        {"🧃","juice drink"},
        {"🥙","wrap food eat meal"},
        {"🥣","bowl cereal breakfast food"},
        {"🧆","falafel food eat"},
        {"🍱","bento lunch food eat"},
        {"🫙","jar food"},
        {"🧈","butter food"},
        {"🥩","meat protein food eat"},
        {"🍳","cook cooking egg breakfast food"},
        {"🫖","teapot tea drink"},
        {"🍶","sake drink"},
        // Mind & Learning
        {"📚","book read learning study"},
        {"📖","book read learning study"},
        {"✏️","pencil write study"},
        {"🎓","graduate study education learn"},
        {"🧠","brain think mental mind"},
        {"💡","idea think light"},
        {"🔬","science research lab microscope"},
        {"🔭","telescope space science"},
        {"📝","write note journal study"},
        {"📊","chart data analysis"},
        {"💻","computer code work laptop"},
        {"📱","phone mobile app"},
        {"🎨","art paint creative draw"},
        {"🎵","music listen audio"},
        {"🎸","guitar music play"},
        {"🎹","piano keyboard music"},
        {"🎤","sing microphone music"},
        {"📸","photo camera photography"},
        {"🎬","film movie video"},
        {"🎭","theater art performance"},
        {"🖊️","pen write journal"},
        {"📐","ruler measure design"},
        {"📏","ruler measure design"},
        {"🗂️","folder organize files"},
        {"📌","pin note"},
        {"🔖","bookmark save read"},
        {"📓","notebook journal write"},
        {"🗒️","notepad write journal"},
        {"🧮","calculator math"},
        {"🎼","music sheet compose"},
        // Nature & Animals
        {"🌱","plant grow garden nature"},
        {"🌿","leaf plant nature green"},
        {"🍀","clover luck nature green"},
        {"🌺","flower nature bloom"},
        {"🌸","blossom flower nature spring"},
        {"🌻","sunflower flower nature"},
        {"🌳","tree nature outdoor"},
        {"🌊","ocean wave water nature"},
        {"🏔️","mountain nature outdoor hike"},
        {"🌅","sunrise sunset nature morning"},
        {"🦋","butterfly nature beauty"},
        {"🐝","bee nature garden"},
        {"🌾","wheat grain nature"},
        {"🍂","leaf autumn fall nature"},
        {"🌈","rainbow nature color"},
        {"❄️","snow winter cold ice"},
        {"🌙","moon night sleep"},
        {"☀️","sun sunny warm morning"},
        {"⭐","star achievement goal"},
        {"🌟","star achievement shine"},
        {"🐶","dog pet animal"},
        {"🐱","cat pet animal"},
        {"🐸","frog animal nature"},
        {"🦁","lion animal strength"},
        {"🦊","fox animal nature"},
        {"🦅","eagle bird nature freedom"},
        {"🐠","fish aquatic nature"},
        {"🐢","turtle animal nature slow"},
        {"🦔","hedgehog animal cute"},
        {"🌵","cactus plant desert"},
        // Home & Daily Life
        {"🏠","home house clean"},
        {"🧹","clean sweep chore home"},
        {"🧺","laundry clean chore wash"},
        {"🪴","plant home indoor garden"},
        {"🛒","shop grocery store errand"},
        {"🧑‍🍳","cook chef cooking food"},
        {"🧽","clean sponge wash"},
        {"🪟","window clean"},
        {"🗓️","calendar schedule plan"},
        {"⏰","alarm wake morning routine"},
        {"🛁","bath wash clean relax"},
        {"🪥","brush teeth hygiene dental"},
        {"🧴","skincare hygiene routine"},
        {"🪞","mirror look grooming"},
        {"🛋️","sofa rest relax home"},
        {"🧸","toy play comfort"},
        {"🖼️","art picture home"},
        {"🪑","chair sit home"},
        {"🛏️","bed sleep rest"},
        {"🪣","bucket clean"},
        // Goals & Achievement
        {"🎯","goal target aim focus"},
        {"✅","done complete check success"},
        {"🏆","trophy win achieve success"},
        {"🥇","medal gold win achieve"},
        {"🚀","launch goal achieve dream"},
        {"⚡","energy fast power"},
        {"🔥","fire streak hot motivated"},
        {"💰","money save finance wealth"},
        {"📈","growth progress improve"},
        {"💎","gem diamond value"},
        {"🗺️","map explore adventure"},
        {"🔑","key unlock achieve"},
        {"🎁","gift reward"},
        {"🎊","celebrate party"},
        {"🎉","celebrate party success"},
        {"🏅","medal achieve award"},
        {"🌠","star wish dream"},
        {"💫","star sparkle achievement"},
        {"✨","sparkle shine achievement"},
        {"🎖️","medal honor achieve"},
        // Travel & Transport
        {"✈️","fly plane travel"},
        {"🚗","car drive commute travel"},
        {"🚲","bike cycle commute travel"},
        {"🎒","bag travel hike"},
        {"🏖️","beach vacation relax travel"},
        {"⛰️","mountain hike outdoor nature"},
        {"🌍","world earth travel"},
        {"🚌","bus commute travel"},
        {"🛤️","track road travel"},
        {"⛵","sail boat water travel"},
        // Wellbeing & Mood
        {"😊","smile happy mood positive"},
        {"🥰","love happy mood"},
        {"😎","cool confident mood"},
        {"🙏","gratitude thankful prayer"},
        {"😴","sleep rest"},
        {"🛌","sleep bed rest"},
        {"🧖","spa relax self care"},
        {"💆","massage relax self care"},
        {"🫂","hug support social"},
        {"❤️","love heart care"},
        {"💜","purple heart love"},
        {"💚","green heart health"},
        {"💙","blue heart calm"},
        {"🤍","white heart pure"},
        {"🕊️","peace dove calm"},
        {"🫶","love heart care"},
        {"🌼","flower bloom spring happy"},
        {"🍃","leaf nature calm"},
        {"🫧","bubble fresh clean"},
    };

    public static readonly List<string> ColorOptions = new()
    {
        "Yellow", "Orange", "Red", "Pink", "Purple", "Blue", "Green", "Black", "White",
        "PastelYellow", "PastelOrange", "PastelRed", "PastelPink",
        "PastelPurple", "PastelBlue", "PastelGreen", "PastelTeal"
    };

    public static readonly List<string> EmojiOptions = new()
    {
        // Fitness & Sport
        "🏃", "🚶", "💪", "🧘", "🏋️", "🤸", "🚴", "🏊", "🧗", "⛹️",
        "🤾", "🏇", "🤽", "🏂", "⛷️", "🤺", "🥊", "🎽", "🥋", "🏄",
        "🧜", "🪂", "🤿", "🎿", "🛹", "🏌️", "🏹", "🎣", "🤼", "🚣",
        // Food & Drink
        "🍎", "🥗", "🥦", "🍵", "☕", "💧", "🥤", "🍇", "🍓", "🥑",
        "🍌", "🍊", "🥕", "🥚", "🫐", "🥝", "🍋", "🥜", "🧃", "🧋",
        "🥙", "🥣", "🧆", "🍱", "🫙", "🧈", "🥩", "🍳", "🫖", "🍶",
        // Mind & Learning
        "📚", "📖", "✏️", "🎓", "🧠", "💡", "🔬", "🔭", "📝", "📊",
        "💻", "📱", "🎨", "🎵", "🎸", "🎹", "🎤", "📸", "🎬", "🎭",
        "🖊️", "📐", "📏", "🗂️", "📌", "🔖", "📓", "🗒️", "🧮", "🎼",
        // Nature & Animals
        "🌱", "🌿", "🍀", "🌺", "🌸", "🌻", "🌳", "🌊", "🏔️", "🌅",
        "🦋", "🐝", "🌾", "🍂", "🌈", "❄️", "🌙", "☀️", "⭐", "🌟",
        "🐶", "🐱", "🐸", "🦁", "🦊", "🦅", "🐠", "🐢", "🦔", "🌵",
        // Home & Daily Life
        "🏠", "🧹", "🧺", "🪴", "🛒", "🧑‍🍳", "🧽", "🪟", "🗓️", "⏰",
        "🛁", "🪥", "🧴", "🪞", "🛋️", "🧸", "🖼️", "🪑", "🛏️", "🪣",
        // Goals & Achievement
        "🎯", "✅", "🏆", "🥇", "🚀", "⚡", "🔥", "💰", "📈", "💎",
        "🗺️", "🔑", "🎁", "🎊", "🎉", "🏅", "🌠", "💫", "✨", "🎖️",
        // Travel & Transport
        "✈️", "🚗", "🚲", "🎒", "🏖️", "⛰️", "🌍", "🚌", "🛤️", "⛵",
        // Wellbeing & Mood
        "😊", "🥰", "😎", "🙏", "😴", "🛌", "🧖", "💆", "🫂", "❤️",
        "💜", "💚", "💙", "🤍", "🕊️", "🫶", "🪷", "🌼", "🍃", "🫧",
    };

    public HabitEditViewModel(IHabitService habitService)
    {
        _habitService = habitService;
    }

    partial void OnHabitIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var habits = await _habitService.GetHabitsAsync();
        _habit = habits.FirstOrDefault(h => h.Id == HabitId);
        if (_habit is null) return;

        Name = _habit.Name;
        Icon = _habit.Icon;
        BackgroundColor = _habit.BackgroundColor;
        BackgroundImageUrl = _habit.BackgroundImageUrl;
        AccentColor = _habit.AccentColor;
    }

    [RelayCommand]
    private void SelectIcon(string emoji) => Icon = emoji;

    [RelayCommand]
    private void SelectBackgroundColor(string colorName)
    {
        BackgroundColor = colorName;
        BackgroundImageUrl = null;
    }

    [RelayCommand]
    private void SelectAccentColor(string colorName) => AccentColor = colorName;

    [RelayCommand]
    private async Task SearchBackgroundAsync()
    {
        var query = BackgroundSearch.Trim();
        if (string.IsNullOrEmpty(query)) return;
        try
        {
            var url = $"https://api.unsplash.com/search/photos?query={Uri.EscapeDataString(query)}&per_page=20&client_id={UnsplashKey}";
            var json = await _http.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<UnsplashResponse>(json, _jsonOpts);
            BackgroundPhotos.Clear();
            foreach (var r in result?.Results ?? [])
                BackgroundPhotos.Add(new UnsplashPhoto(r.Urls.Small, r.Urls.Regular));
            HasBackgroundPhotos = BackgroundPhotos.Count > 0;
        }
        catch { /* silently leave existing state intact */ }
    }

    [RelayCommand]
    private void SelectBackgroundImage(UnsplashPhoto photo)
    {
        BackgroundImageUrl = photo.RegularUrl;
        BackgroundColor = null;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (_habit is null) return;

        _habit.Name = string.IsNullOrWhiteSpace(Name) ? "Untitled" : Name.Trim();
        _habit.Icon = string.IsNullOrWhiteSpace(Icon) ? null : Icon.Trim();

        if (!string.IsNullOrWhiteSpace(BackgroundImageUrl))
        {
            _habit.BackgroundImageUrl = BackgroundImageUrl.Trim();
            _habit.BackgroundColor = null;
        }
        else
        {
            _habit.BackgroundImageUrl = null;
            _habit.BackgroundColor = BackgroundColor;
        }

        _habit.AccentColor = AccentColor;

        await _habitService.SaveHabitAsync(_habit);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (_habit is null) return;
        var confirmed = await Shell.Current.DisplayAlert(
            "Delete Habit",
            $"Delete \"{Name}\"? This cannot be undone.",
            "Delete", "Cancel");

        if (!confirmed) return;
        await _habitService.DeleteHabitAsync(_habit.Id);
        await Shell.Current.GoToAsync("../..");
    }
}

file record UnsplashResponse(List<UnsplashResult> Results);
file record UnsplashResult(UnsplashUrls Urls);
file record UnsplashUrls(string Small, string Regular);
