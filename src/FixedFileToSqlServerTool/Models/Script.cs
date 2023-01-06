using LiteDB;

namespace FixedFileToSqlServerTool.Models;

public record class Script
{
    private const string DefaultCode = @"using System;
// テストデータは、Value変数から取得します。Valueの型はString
//
// 取得結果はreturn句を使用

return Value;";

    public required ObjectId Id { get; init; }

    public required string Name { get; init; }

    public required string Code { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime CreatedAt { get; init; }

    public static Script Create(string name) =>
        new Script
        {
            Id = ObjectId.NewObjectId(),
            Name = name,
            Code = DefaultCode,
            UpdatedAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };

    public Script Renew(string name, string code) =>
        this with
        {
            Name = name,
            Code = code,
            UpdatedAt = DateTime.Now
        };
}
