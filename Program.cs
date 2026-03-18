var builder = WebApplication.CreateBuilder(args);

// --- חלק 1: רישום שירותים (Services) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// הגדרת CORS - חייב להירשם כאן, לפני ה-Build!
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => {
        policy.WithOrigins("http://localhost:4200") // הכתובת של האנגולר שלך
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// --- חלק 2: הגדרת צינור הבקשות (Middleware Pipeline) ---
// הסדר כאן קריטי!

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. הפנייה ל-HTTPS (אופציונלי בפיתוח מקומי אבל כדאי שיהיה)
app.UseHttpsRedirection();

// 2. הפעלת ה-CORS (חייב לבוא לפני Authorization)
app.UseCors("AllowAngular");

// 3. אישור הרשאות (כרגע אין לנו יוזרים, אבל זה חלק מהשלד)
app.UseAuthorization();

// 4. מיפוי הקונטרולרים (כדי שהשרת ידע לאן לנתב כל בקשה)
app.MapControllers();

// 5. הפעלת השרת
app.Run();