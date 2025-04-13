using CollegeTrackAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CollegeTrackAPI.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {
            // Ensure DB is created
            //context.Database.Migrate();

            if (context.Tracks.Any())
                return; // DB has been seeded

            var tracksData = new List<Track>
{
    new Track
    {
        Title = "هندسة البرمجيات",
        ShortDescription = "تطوير مهارات هندسة البرمجيات المتقدمة مع التركيز على تطوير التطبيقات وتصميم الأنظمة البرمجية.",
        FullDescription = "تم تصميم برنامج هندسة البرمجيات لدينا لإعداد الطلاب لوظائف في تطوير البرمجيات وتصميم الأنظمة وإدارة مشاريع البرمجيات. يغطي المنهج مبادئ هندسة البرمجيات الأساسية وتقنيات التطوير المتقدمة ومجالات متخصصة مثل الذكاء الاصطناعي وعلوم البيانات والتطوير السحابي والأمن السيبراني.",
        Duration = "٤ سنوات",
        CareerOutlook = "يتمتع خريجو برنامج هندسة البرمجيات لدينا بآفاق وظيفية ممتازة في تطوير البرمجيات وهندسة الأنظمة وعلوم البيانات والذكاء الاصطناعي. يستمر الطلب على مهندسي البرمجيات المهرة في النمو عبر مختلف الصناعات.",
        Image = "https://images.unsplash.com/photo-1461749280684-dccba630e2f6?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1770&q=80",
        Requirements = new List<string>
        {
            "شهادة الثانوية العامة أو ما يعادلها",
            "معدل تراكمي لا يقل عن ٣.٠",
            "إتمام مادة ما قبل حساب التفاضل والتكامل",
            "معرفة أساسية بالبرمجة (مستحسن)"
        },
        Courses = new List<Course>
        {
            new Course { CourseCode = "se101", Title = "أساسيات البرمجة", Description = "أساسيات البرمجة باستخدام بايثون وجافا، وتغطية المتغيرات وهياكل التحكم والوظائف وهياكل البيانات الأساسية.", Credits = 4 },
            new Course { CourseCode = "se201", Title = "هياكل البيانات والخوارزميات", Description = "دراسة هياكل البيانات والخوارزميات الأساسية، بما في ذلك تحليل تعقيد الوقت والمساحة.", Credits = 4 },
            new Course { CourseCode = "se310", Title = "هندسة قواعد البيانات", Description = "تصميم وتنفيذ أنظمة قواعد البيانات، بما في ذلك قواعد البيانات العلائقية وSQL وNoSQL.", Credits = 3 },
            new Course { CourseCode = "se450", Title = "تطوير تطبيقات الذكاء الاصطناعي", Description = "تطبيق مفاهيم الذكاء الاصطناعي في بناء التطبيقات، بما في ذلك التعلم الآلي والشبكات العصبية ومعالجة اللغة الطبيعية.", Credits = 4 }
        }
    },
    new Track
    {
        Title = "إدارة المشروعات",
        ShortDescription = "اكتساب معرفة شاملة في إدارة المشروعات والتخطيط الاستراتيجي والقيادة وتحليل الأعمال.",
        FullDescription = "يوفر برنامج إدارة المشروعات للطلاب أساسًا متينًا في مبادئ وممارسات إدارة المشاريع الحديثة. يطور الطلاب مهارات في التخطيط الاستراتيجي وإدارة الموارد وتقييم المخاطر وتطبيق منهجيات أجايل وسكرم، مما يؤهلهم لقيادة المشاريع بكفاءة عالية. يركز المنهج على كل من المعرفة النظرية والتطبيق العملي من خلال دراسات الحالة والمشاريع والتدريب العملي.",
        Duration = "٣ سنوات",
        CareerOutlook = "يتابع خريجو إدارة المشروعات وظائف في إدارة المشاريع والتخطيط الاستراتيجي والاستشارات والقيادة التنفيذية. كما يواصل العديد من الخريجين دراساتهم العليا في برامج متخصصة لتعزيز آفاقهم المهنية بشكل أكبر.",
        Image = "https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1770&q=80",
        Requirements = new List<string>
        {
            "شهادة الثانوية العامة أو ما يعادلها",
            "معدل تراكمي لا يقل عن ٢.٥",
            "إتمام الجبر ٢",
            "اهتمام بإدارة المشاريع والاقتصاد"
        },
        Courses = new List<Course>
        {
            new Course { CourseCode = "pm101", Title = "أساسيات إدارة المشروعات", Description = "مقدمة في نظريات إدارة المشروعات ودورة حياة المشروع ومبادئ القيادة.", Credits = 3 },
            new Course { CourseCode = "pm210", Title = "التخطيط الاستراتيجي", Description = "نظرة عامة على عمليات التخطيط الاستراتيجي وإدارة الموارد وتحديد الأولويات.", Credits = 3 },
            new Course { CourseCode = "pm330", Title = "إدارة المخاطر في المشاريع", Description = "مبادئ تحديد وتقييم وإدارة المخاطر في سياق المشاريع المختلفة.", Credits = 4 },
            new Course { CourseCode = "pm450", Title = "منهجيات أجايل وسكرم", Description = "دراسة متقدمة في تطبيق منهجيات أجايل وسكرم في إدارة المشاريع المعاصرة.", Credits = 3 }
        }
    },
    new Track
    {
        Title = "إدارة المخاطر والأزمات",
        ShortDescription = "الاستعداد لأدوار قيادية في إدارة المخاطر والأزمات مع منهجيات متخصصة في تقييم وإدارة المخاطر.",
        FullDescription = "يجمع برنامج إدارة المخاطر والأزمات بين نظريات إدارة المخاطر وتقنيات إدارة الأزمات مع مبادئ إدارية متقدمة. يطور الطلاب خبرة في تحليل المخاطر واستراتيجيات التخفيف والاستجابة للأزمات وإدارة الكوارث.",
        Duration = "٣ سنوات",
        CareerOutlook = "يتم إعداد خريجي برنامج إدارة المخاطر والأزمات لشغل وظائف كمديري مخاطر ومحللي أزمات ومخططي استمرارية أعمال واستشاريين في مجال الأمن والسلامة.",
        Image = "https://images.unsplash.com/photo-1504439468489-c8920d796a29?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8&auto=format&fit=crop&w=1771&q=80",
        Requirements = new List<string>
        {
            "شهادة الثانوية العامة أو ما يعادلها",
            "معدل تراكمي لا يقل عن ٢.٨",
            "إتمام مادتي الرياضيات والإحصاء",
            "اهتمام بإدارة المخاطر والأزمات"
        },
        Courses = new List<Course>
        {
            new Course { CourseCode = "rm101", Title = "أساسيات إدارة المخاطر", Description = "نظرة عامة على مفاهيم وأطر إدارة المخاطر وتقييم المخاطر النوعي والكمي.", Credits = 3 },
            new Course { CourseCode = "rm220", Title = "إدارة الأزمات والكوارث", Description = "مفاهيم وأدوات الاستعداد للأزمات والاستجابة لها والتعافي منها.", Credits = 4 },
            new Course { CourseCode = "rm340", Title = "نظم المعلومات وإدارة المخاطر", Description = "تطبيق تكنولوجيا المعلومات في تحليل المخاطر ونمذجتها وإدارتها.", Credits = 3 },
            new Course { CourseCode = "rm460", Title = "استمرارية الأعمال والقدرة على الصمود", Description = "مبادئ وطرق تطوير خطط استمرارية الأعمال وبناء المنظمات القادرة على الصمود.", Credits = 3 }
        }
    }
};


            context.Tracks.AddRange(tracksData);
            context.SaveChanges();
        }
    }
}
