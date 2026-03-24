using Sprint_1.DTOs;
using Sprint_1.Exceptions;
using Sprint_1.Models;
using Sprint_1.Services;

namespace Sprint_1.Tests;

public class EventServiceTests
{
    private readonly EventService _service = new();

    private static Event CreateValidEvent(string title = "Test Event", int startHoursFromNow = 1, int durationHours = 2)
    {
        return new Event
        {
            Title = title,
            Description = "Test description",
            StartAt = DateTime.UtcNow.AddHours(startHoursFromNow),
            EndAt = DateTime.UtcNow.AddHours(startHoursFromNow + durationHours)
        };
    }

    private static EventFilterDto DefaultFilter => new();

    // ===== Успешные сценарии =====

    [Fact]
    public void Create_ValidEvent_ReturnsCreatedEvent()
    {
        var ev = CreateValidEvent();
        var result = _service.Create(ev);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(ev.Title, result.Title);
        Assert.Equal(ev.Description, result.Description);
    }

    [Fact]
    public void GetAll_ReturnsAllEvents()
    {
        _service.Create(CreateValidEvent("Event 1"));
        _service.Create(CreateValidEvent("Event 2"));

        var result = _service.GetAll(DefaultFilter);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public void GetById_ExistingId_ReturnsEvent()
    {
        var created = _service.Create(CreateValidEvent());
        var result = _service.GetById(created.Id);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal(created.Title, result.Title);
    }

    [Fact]
    public void Update_ExistingEvent_ReturnsUpdatedEvent()
    {
        var created = _service.Create(CreateValidEvent());
        var updated = CreateValidEvent("Updated Title");

        var result = _service.Update(created.Id, updated);

        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Updated Title", result.Title);
    }

    [Fact]
    public void Delete_ExistingEvent_RemovesEvent()
    {
        var created = _service.Create(CreateValidEvent());

        _service.Delete(created.Id);

        Assert.Throws<NotFoundException>(() => _service.GetById(created.Id));
    }

    // ===== Фильтрация =====

    [Fact]
    public void GetAll_FilterByTitle_ReturnsMatchingEvents()
    {
        _service.Create(CreateValidEvent("Sprint Planning"));
        _service.Create(CreateValidEvent("Daily Standup"));
        _service.Create(CreateValidEvent("Sprint Review"));

        var filter = new EventFilterDto { Title = "sprint" };
        var result = _service.GetAll(filter);

        Assert.Equal(2, result.TotalCount);
    }

    [Fact]
    public void GetAll_FilterByFrom_ReturnsEventsStartingAfter()
    {
        _service.Create(CreateValidEvent("Past", -10, 1));
        _service.Create(CreateValidEvent("Future", 10, 1));

        var filter = new EventFilterDto { From = DateTime.UtcNow };
        var result = _service.GetAll(filter);

        Assert.Single(result.Items);
        Assert.Equal("Future", result.Items.First().Title);
    }

    [Fact]
    public void GetAll_FilterByTo_ReturnsEventsEndingBefore()
    {
        _service.Create(CreateValidEvent("Soon", 1, 1));
        _service.Create(CreateValidEvent("Later", 100, 1));

        var filter = new EventFilterDto { To = DateTime.UtcNow.AddHours(50) };
        var result = _service.GetAll(filter);

        Assert.Single(result.Items);
        Assert.Equal("Soon", result.Items.First().Title);
    }

    [Fact]
    public void GetAll_CombinedFilters_WorksTogether()
    {
        _service.Create(CreateValidEvent("Sprint Planning", 1, 1));
        _service.Create(CreateValidEvent("Sprint Review", 100, 1));
        _service.Create(CreateValidEvent("Daily Standup", 1, 1));

        var filter = new EventFilterDto
        {
            Title = "Sprint",
            From = DateTime.UtcNow,
            To = DateTime.UtcNow.AddHours(50)
        };
        var result = _service.GetAll(filter);

        Assert.Single(result.Items);
        Assert.Equal("Sprint Planning", result.Items.First().Title);
    }

    // ===== Пагинация =====

    [Fact]
    public void GetAll_Pagination_ReturnsCorrectPage()
    {
        for (var i = 0; i < 25; i++)
            _service.Create(CreateValidEvent($"Event {i}", i + 1, 1));

        var filter = new EventFilterDto { Page = 2, PageSize = 10 };
        var result = _service.GetAll(filter);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public void GetAll_LastPage_ReturnsRemainingItems()
    {
        for (var i = 0; i < 25; i++)
            _service.Create(CreateValidEvent($"Event {i}", i + 1, 1));

        var filter = new EventFilterDto { Page = 3, PageSize = 10 };
        var result = _service.GetAll(filter);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(5, result.Items.Count);
    }

    [Fact]
    public void GetAll_EmptyCollection_ReturnsEmptyResult()
    {
        var result = _service.GetAll(DefaultFilter);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    // ===== Сценарии ошибок =====

    [Fact]
    public void GetById_NonExistentId_ThrowsNotFoundException()
    {
        var ex = Assert.Throws<NotFoundException>(() => _service.GetById(Guid.NewGuid()));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void Update_NonExistentId_ThrowsNotFoundException()
    {
        var ev = CreateValidEvent();
        var ex = Assert.Throws<NotFoundException>(() => _service.Update(Guid.NewGuid(), ev));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void Delete_NonExistentId_ThrowsNotFoundException()
    {
        var ex = Assert.Throws<NotFoundException>(() => _service.Delete(Guid.NewGuid()));
        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void Create_EndAtBeforeStartAt_ThrowsValidationException()
    {
        var ev = new Event
        {
            Title = "Invalid",
            StartAt = DateTime.UtcNow.AddHours(5),
            EndAt = DateTime.UtcNow.AddHours(1)
        };

        var ex = Assert.Throws<Exceptions.ValidationException>(() => _service.Create(ev));
        Assert.Contains("EndAt", ex.Message);
    }

    [Fact]
    public void Update_EndAtBeforeStartAt_ThrowsValidationException()
    {
        var created = _service.Create(CreateValidEvent());
        var invalid = new Event
        {
            Title = "Invalid",
            StartAt = DateTime.UtcNow.AddHours(5),
            EndAt = DateTime.UtcNow.AddHours(1)
        };

        var ex = Assert.Throws<Exceptions.ValidationException>(() => _service.Update(created.Id, invalid));
        Assert.Contains("EndAt", ex.Message);
    }

    // ===== Граничные случаи =====

    [Fact]
    public void GetAll_OrdersByStartAt()
    {
        _service.Create(CreateValidEvent("Later", 10, 1));
        _service.Create(CreateValidEvent("Sooner", 1, 1));

        var result = _service.GetAll(DefaultFilter);

        Assert.Equal("Sooner", result.Items.First().Title);
        Assert.Equal("Later", result.Items.Last().Title);
    }

    [Fact]
    public void Create_ReturnsClonedEvent_NotSameReference()
    {
        var original = CreateValidEvent();
        var created = _service.Create(original);

        original.Title = "Modified";

        var fetched = _service.GetById(created.Id);
        Assert.NotEqual("Modified", fetched.Title);
    }
}
