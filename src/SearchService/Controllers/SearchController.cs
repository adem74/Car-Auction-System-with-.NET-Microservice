using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Search;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SeachItems([FromQuery] SearchQuery seachQuery)
    {
        var query = DB.PagedSearch<Item, Item>();
        if (!string.IsNullOrEmpty(seachQuery.SearchTerm))
        {
            query.Match(Search.Full, seachQuery.SearchTerm).SortByTextScore();
        }
        query = seachQuery.OrderBy switch
        {
            "make"
                => query.Sort(x => x.Ascending(a => a.Make)).Sort(x => x.Ascending(a => a.Model)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd)),
        };

        query = seachQuery.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon"
                => query.Match(
                    x =>
                        x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow
                ),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrEmpty(seachQuery.Seller))
        {
            query.Match(x => x.Seller == seachQuery.Seller);
        }
        if (!string.IsNullOrEmpty(seachQuery.Winner))
        {
            query.Match(x => x.Winner == seachQuery.Winner);
        }
        query.PageNumber(seachQuery.PageNumber);
        query.PageSize(seachQuery.PageSize);
        var result = await query.ExecuteAsync();
        return Ok(
            new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            }
        );
    }
}
