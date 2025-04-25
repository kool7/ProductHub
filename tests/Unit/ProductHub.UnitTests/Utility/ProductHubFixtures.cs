using AutoFixture;

namespace ProductHub.UnitTests.Utility
{
    internal static class ProductHubFixtures
    {
        private static Fixture _fixture = new Fixture();

        public static (int pageNumber, int pageSize, string searchTerm, string sort) GeneratePaginationData(PaginationScenario scenario)
        {
            int pageNumber, pageSize;
            string searchTerm, sort;

            var term = _fixture.Create<string>()[..3];

            var sortOrder = _fixture.Create<bool>() ? "asc" : "desc";
            var number = Math.Clamp(_fixture.Create<int>(), 1, 5);
            var inValidPagesizeAndNum = Math.Clamp(_fixture.Create<int>(), -10, 0);

            switch (scenario)
            {
                case PaginationScenario.DefaultParameters:
                    pageNumber = 1;
                    pageSize = 10;
                    searchTerm = "";
                    sort = "asc";
                    break;
                case PaginationScenario.AllValid:
                    pageNumber = number;
                    pageSize = 10;
                    searchTerm = term;
                    sort = sortOrder;
                    break;
                case PaginationScenario.InvalidPageNumber:
                    pageNumber = inValidPagesizeAndNum;
                    pageSize = 10;
                    searchTerm = term;
                    sort = sortOrder;
                    break;
                case PaginationScenario.InvalidPageSize:
                    pageNumber = number;
                    pageSize = inValidPagesizeAndNum;
                    searchTerm = term;
                    sort = sortOrder;
                    break;
                case PaginationScenario.InvalidSearchTerm:
                    pageNumber = number;
                    pageSize = 10;
                    searchTerm = null;
                    sort = sortOrder;
                    break;
                case PaginationScenario.InvalidSortOrder:
                    pageNumber = number;
                    pageSize = 10;
                    searchTerm = term;
                    sort = _fixture.Create<bool>() ? "invalid_sort" : "invalid_sort";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, "Invalid scenario");
            }

            return (pageNumber, pageSize, searchTerm, sort);
        }

        public enum PaginationScenario
        {
            AllValid,
            InvalidPageNumber,
            InvalidPageSize,
            InvalidSearchTerm,
            InvalidSortOrder,
            DefaultParameters
        }
    }
}
