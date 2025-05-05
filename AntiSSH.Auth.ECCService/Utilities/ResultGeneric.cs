using AntiSSH.Auth.ECC.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AntiSSH.Auth.ECC.Utilities;

public class Result<T> : Result
{
    public T? Data { get; set; }

    public override IActionResult GetActionResult()
    {
        return Code == HttpCode.Ok ? new OkObjectResult(Data) : base.GetActionResult();
    }
}