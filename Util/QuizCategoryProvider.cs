using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Util
{
    public static class QuizCategoryProvider
    {
        private static List<QuizCategoryDto> _categories = new();

        private static Dictionary<int, QuizCategoryDto> _byId;
        private static Dictionary<string, QuizCategoryDto> _byName;
        private static ILookup<int?, QuizCategoryDto> _bySuperCategoryId;

        static QuizCategoryProvider()
        {
            RefreshLookups();
        }

        private static void RefreshLookups()
        {
            _byId = _categories.ToDictionary(c => c.Id);
            _byName = _categories.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            _bySuperCategoryId = _categories.ToLookup(c => c.SuperCategoryId);
        }

        public static QuizCategoryDto? GetById(int id) => _byId.TryGetValue(id, out var category) ? category : null;

        public static QuizCategoryDto? GetByName(string name) => _byName.TryGetValue(name, out var category) ? category : null;

        public static IEnumerable<QuizCategoryDto> GetBySuperCategoryId(int? superCategoryId) => _bySuperCategoryId[superCategoryId];

        public static void AddCategory(QuizCategoryDto newCategory)
        {
            _categories.Add(newCategory);
            RefreshLookups();
        }

        public static IEnumerable<QuizCategoryDto> GetAll() => _categories;
    }
}
