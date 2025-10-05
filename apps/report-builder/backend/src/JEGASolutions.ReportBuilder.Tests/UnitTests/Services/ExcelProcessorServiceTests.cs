public class ExcelProcessorServiceTests
{
    [Fact]
    public async Task ProcessExcelFileAsync_ValidExcel_ExtractsDataCorrectly()
    {
        // Arrange
        var service = new ExcelProcessorService();
        var base64Excel = GenerateTestExcelBase64();

        // Act
        var result = await service.ProcessExcelFileAsync(base64Excel);

        // Assert
        result.Should().NotBeNull();
        result.Rows.Should().BeGreaterThan(0);
        result.Columns.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ProcessExcelFileAsync_InvalidBase64_ThrowsException()
    {
        // Arrange
        var service = new ExcelProcessorService();
        var invalidBase64 = "invalid-base64-string";

        // Act & Assert
        await Assert.ThrowsAsync<FormatException>(() => 
            service.ProcessExcelFileAsync(invalidBase64)
        );
    }

    private string GenerateTestExcelBase64()
    {
        // Helper method to create test Excel file
        // Implementation here...
        return Convert.ToBase64String(new byte[] { /* Excel bytes */ });
    }
}