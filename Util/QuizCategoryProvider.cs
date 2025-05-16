using PubQuizBackend.Exceptions;
using PubQuizBackend.Model.Dto.QuizCategoryDto;

namespace PubQuizBackend.Util
{
    public static class QuizCategoryProvider
    {
        private static List<QCategoryDto> _categories = new();

        private static Dictionary<int, QCategoryDto> _byId = new();
        private static Dictionary<string, QCategoryDto> _byName = new();
        private static ILookup<int?, QCategoryDto> _bySuperCategoryId = Enumerable.Empty<QCategoryDto>().ToLookup(c => c.SuperCategoryId);

        public static void Initialize(List<QCategoryDto> categories)
        {
            _categories = categories;
            RefreshLookups();
        }

        private static void RefreshLookups()
        {
            _byId = _categories.ToDictionary(c => c.Id);
            _byName = _categories.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            _bySuperCategoryId = _categories.ToLookup(c => c.SuperCategoryId);
        }

        public static QCategoryDto GetById(int id) =>
            _byId.TryGetValue(id, out var category)
                ? category
                : throw new NotFoundException("Category not found!");

        public static IEnumerable<QCategoryDto> GetBySimilarName(string name)
        {
            return _byName.Values
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<QCategoryDto> GetBySuperCategoryId(int? superCategoryId) => _bySuperCategoryId[superCategoryId];

        public static void AddCategory(QCategoryDto newCategory)
        {
            _categories.Add(newCategory);
            RefreshLookups();
        }

        public static IEnumerable<QCategoryDto> GetAll() => _categories;
    }

}
