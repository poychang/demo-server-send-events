var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

// MDN 使用 server-sent 事件
// https://developer.mozilla.org/zh-TW/docs/Web/API/Server-sent_events/Using_server-sent_events
app.MapGet("api/server-send-events", async (HttpContext context) =>
{
    // APS.NET Core 2.1 以上版本預設使用 HTTP/2 通訊協定，SSE 在 HTTP/1.1 只能有 6 個連線，在 HTTP/2 預設有 100 個
    Console.WriteLine(context.Request.Protocol);

    // 在回應的 Header 上加上 text/event-stream，標示此回應是 SSE
    context.Response.Headers.Add("Content-Type", "text/event-stream");
    for (int i = 0; i < 10; i++)
    {
        // 自定義事件，格式為 event:<事件名稱>，例如 event:ping
        // 結尾要加上換行符號
        await context.Response.WriteAsync($"event:ping\n");

        // 發送串流資料，格式為 data:<資料內容>，例如 data:I am Poy Chang
        // 僅支援 UTF-8 編碼的文字，不支援二進位格式的資料
        // 結尾要加上 兩個換行符號
        await context.Response.WriteAsync($"data: {i} at {DateTime.Now}\n\n");

        // 將當前的 Response 資料送出
        await context.Response.Body.FlushAsync();

        await Task.Delay(1000);
    }

    // 資料發送完畢後關閉連線
    context.Response.Body.Close();
});

app.Run();
