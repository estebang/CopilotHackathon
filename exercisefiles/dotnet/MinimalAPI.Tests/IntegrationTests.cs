namespace IntegrationTests;

public class IntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public IntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ReturnsHelloWorld()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("Hello World!", content);
    }

    [Fact]
    public async Task ValidatePhone_ReturnsTrue_WhenPhoneNumberIsValid()
    {
        // Arrange
        var validPhoneNumber = "+34666777888";

        // Act
        var response = await _client.GetAsync($"/validate-phone?phoneNumber={Uri.EscapeDataString(validPhoneNumber)}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"isValid\":true", content);
        Assert.Contains(validPhoneNumber, content);
    }

    [Theory]
    [InlineData("666777888", "missing +34 prefix")]
    [InlineData("+3466677788", "only 8 digits")]
    [InlineData("+346667778889", "10 digits instead of 9")]
    [InlineData("+44666777888", "wrong country code")]
    [InlineData("+34 666 777 888", "contains spaces")]
    [InlineData("+34-666-777-888", "contains dashes")]
    [InlineData("+34abc123456", "contains letters")]
    [InlineData("", "empty string")]
    [InlineData("+34", "only country code")]
    public async Task ValidatePhone_ReturnsFalse_WhenPhoneNumberIsInvalid(string phoneNumber, string reason)
    {
        // Arrange
        // reason parameter is for test documentation only

        // Act
        var response = await _client.GetAsync($"/validate-phone?phoneNumber={Uri.EscapeDataString(phoneNumber)}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"isValid\":false", content);
    }

    [Fact]
    public async Task DaysBetween_ReturnsCorrectDayCount()
    {
        // Arrange
        var startDate = "2024-01-01";
        var endDate = "2024-01-10";

        // Act
        var response = await _client.GetAsync($"/days-between?startDate={startDate}&endDate={endDate}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"daysBetween\":9", content);
    }

    [Fact]
    public async Task DaysBetween_ReturnsAbsoluteValue_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var startDate = "2024-01-10";
        var endDate = "2024-01-01";

        // Act
        var response = await _client.GetAsync($"/days-between?startDate={startDate}&endDate={endDate}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"daysBetween\":9", content);
    }

    [Fact]
    public async Task DaysBetween_ReturnsZero_WhenSameDate()
    {
        // Arrange
        var date = "2024-01-01";

        // Act
        var response = await _client.GetAsync($"/days-between?startDate={date}&endDate={date}");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"daysBetween\":0", content);
    }
}
