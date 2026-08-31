using System.Text.Json;
using caportal.Models.Entities;

namespace caportal.Services.Repositories
{
    public static class MenuRepository
    {
        private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "menu.json");
        private static List<NavbarMenuItem> _items = new();
        private static readonly object _lock = new();

        static MenuRepository()
        {
            Load();
            if (_items.Count == 0)
            {
                // Populate default menu items
                _items = new List<NavbarMenuItem>
                {
                    new() { Id = 1, DisplayName = "Home", Url = "/", Order = 1, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 2, DisplayName = "Services", Url = "#features", Order = 2, IsActive = true, MegaMenuType = "Services" },
                    new() { Id = 3, DisplayName = "Find an Expert", Url = "/find-expert", Order = 3, IsActive = true, MegaMenuType = "FindExpert" },
                    new() { Id = 4, DisplayName = "How It Works", Url = "#how-it-works", Order = 4, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 5, DisplayName = "Pricing", Url = "#pricing", Order = 5, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 6, DisplayName = "Blog", Url = "/blog", Order = 6, IsActive = true, MegaMenuType = "None" },
                    new() { Id = 7, DisplayName = "Contact", Url = "/contact", Order = 7, IsActive = true, MegaMenuType = "None" }
                };
                Save();
            }
        }

        private static void Load()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(FilePath))
                    {
                        var json = File.ReadAllText(FilePath);
                        _items = JsonSerializer.Deserialize<List<NavbarMenuItem>>(json) ?? new();
                    }
                }
                catch
                {
                    _items = new();
                }
            }
        }

        private static void Save()
        {
            lock (_lock)
            {
                try
                {
                    var json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(FilePath, json);
                }
                catch
                {
                    // Ignore or log
                }
            }
        }

        public static List<NavbarMenuItem> GetAll(bool includeInactive = true)
        {
            lock (_lock)
            {
                var query = _items.AsEnumerable();
                if (!includeInactive)
                {
                    query = query.Where(m => m.IsActive);
                }
                return query.OrderBy(m => m.Order).ToList();
            }
        }

        public static NavbarMenuItem? GetById(int id)
        {
            lock (_lock)
            {
                return _items.FirstOrDefault(m => m.Id == id);
            }
        }

        public static void Add(NavbarMenuItem item)
        {
            lock (_lock)
            {
                item.Id = _items.Count > 0 ? _items.Max(m => m.Id) + 1 : 1;
                _items.Add(item);
                Save();
            }
        }

        public static void Update(NavbarMenuItem updated)
        {
            lock (_lock)
            {
                var existing = _items.FirstOrDefault(m => m.Id == updated.Id);
                if (existing != null)
                {
                    existing.DisplayName = updated.DisplayName;
                    existing.Url = updated.Url;
                    existing.Order = updated.Order;
                    existing.IsActive = updated.IsActive;
                    existing.MegaMenuType = updated.MegaMenuType;
                    existing.OpenInNewTab = updated.OpenInNewTab;
                    Save();
                }
            }
        }

        public static void Delete(int id)
        {
            lock (_lock)
            {
                var item = _items.FirstOrDefault(m => m.Id == id);
                if (item != null)
                {
                    _items.Remove(item);
                    Save();
                }
            }
        }
    }
}
