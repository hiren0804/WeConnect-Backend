// src/WeConnect.Infrastructure/Persistence/DatabaseSeeder.cs
namespace WeConnect.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using WeConnect.Domain.Entities;
using WeConnect.Infrastructure.Services;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        MasterDbContext master,
        TenantDbContextFactory? tenantFactory = null,
        ProvisioningService? provisioning = null)
    {
        if (await master.Tenants.AnyAsync()) return;

        // ── Reliance ──────────────────────────────────────────────────
        var reliance = new Tenant {
            Name       = "Reliance",
            Slug       = "reliance",
            Domain     = "reliance.localhost:5173",
            TemplateId = "template1",
            IsActive   = false,          // stays false until ProvisionTenantDbAsync succeeds
            Status     = "provisioning"
        };
        reliance.Modules = [
            new TenantModule {
                TenantId = reliance.Id,
                ModuleKey = "dashboard", IsEnabled = true, DisplayOrder = 0,
                Pages = [
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "home",
                        PageType = "widget", Route = "/",
                        Widgets = [
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "highlights", WidgetType = "highlights",
                                Col = 4, Height = 595, DisplayOrder = 0, IsVisible = true }
                        ]
                    }
                ]
            },
            new TenantModule {
                Id = Guid.NewGuid(), TenantId = reliance.Id,
                ModuleKey = "blogs", IsEnabled = true, DisplayOrder = 1,
                Pages = [
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "home",
                        PageType = "widget", Route = "/blogs",
                        Widgets = [
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "highlights", WidgetType = "highlights",
                                Col = 4, Height = 595, DisplayOrder = 0, IsVisible = true },
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "blogs", WidgetType = "blogs",
                                Col = 4, Height = 600, DisplayOrder = 1, IsVisible = true },
                        ]
                    },
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "detail",
                        PageType = "listing", Route = "/blogs/:id",
                        CardComponent = "photouicard"
                    }
                ]
            }
        ];

        // ── Tata ──────────────────────────────────────────────────────
        var tata = new Tenant {
            Name       = "Tata",
            Slug       = "tata",
            Domain     = "tata.localhost:5173",
            TemplateId = "template2",
            IsActive   = false,
            Status     = "provisioning"
        };
        tata.Modules = [
            new TenantModule {
                Id = Guid.NewGuid(), TenantId = tata.Id,
                ModuleKey = "dashboard", IsEnabled = true, DisplayOrder = 0,
                Pages = [
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "home",
                        PageType = "widget", Route = "/",
                        Widgets = [
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "highlights", WidgetType = "highlights",
                                Col = 4, Height = 595, DisplayOrder = 0, IsVisible = true }
                        ]
                    }
                ]
            },
            new TenantModule {
                Id = Guid.NewGuid(), TenantId = tata.Id,
                ModuleKey = "events", IsEnabled = true, DisplayOrder = 1,
                Pages = [
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "home",
                        PageType = "widget", Route = "/events",
                        Widgets = [
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "highlights", WidgetType = "highlights",
                                Col = 4, Height = 595, DisplayOrder = 0, IsVisible = true },
                            new TenantWidget { Id = Guid.NewGuid(),
                                WidgetKey = "events", WidgetType = "events",
                                Col = 4, Height = 600, DisplayOrder = 1, IsVisible = true },
                        ]
                    },
                    new TenantPage {
                        Id = Guid.NewGuid(), PageKey = "detail",
                        PageType = "listing", Route = "/events/:id",
                        CardComponent = "photouicard"
                    }
                ]
            }
        ];

        // Save tenant rows first — provisioning needs the IDs to exist in master
        master.Tenants.AddRange(reliance, tata);
        await master.SaveChangesAsync();

        // ── Provision each tenant DB (clone + migrate + folder) ───────
        // ProvisionTenantDbAsync creates the DB, stores connection, runs migrations
        // It does NOT re-insert the tenant row — that's already done above
        if (provisioning is not null)
        {
            await ProvisionAndSeedAsync(provisioning, tenantFactory, reliance, "blog");
            await ProvisionAndSeedAsync(provisioning, tenantFactory, tata,     "event");
        }
        else
        {
            Console.WriteLine("[Seeder] ProvisioningService not provided — skipping DB clone.");
        }
    }

    private static async Task ProvisionAndSeedAsync(
        ProvisioningService provisioning,
        TenantDbContextFactory? tenantFactory,
        Tenant tenant,
        string contentType)
    {
        try
        {
            // This clones weconnect_base → weconnect_{slug},
            // stores TenantConnection in master, runs EF migrations
            await provisioning.ProvisionTenantDbAsync(tenant);

            // Mark active only after DB is confirmed ready
            tenant.IsActive = true;
            tenant.Status   = "active";

            // Seed content into the new tenant DB
            if (tenantFactory is not null)
                await SeedTenantContentAsync(tenantFactory, tenant, contentType);
        }
        catch (Exception ex)
        {
            // Status already set to failed inside ProvisionTenantDbAsync
            Console.WriteLine($"[Seeder] Provisioning failed for '{tenant.Slug}': {ex.Message}");
        }
    }

    private static async Task SeedTenantContentAsync(
        TenantDbContextFactory factory,
        Tenant tenant,
        string contentType)
    {
        try
        {
            await using var db = await factory.CreateAsync(tenant.Slug);

            if (await db.ContentItems.AnyAsync()) return;

            var items = new[] {
                ("TATA Projects wins order for bioethanol plant in India",
                 "This article really breaks down the key principles of financial management.",
                 "This blog is a great resource for anyone who feels overwhelmed by financial planning.",
                 128, 34, 12),
                ("Adani Green to develop world's largest renewable energy park",
                 "This article really breaks down the key principles of financial management.",
                 "This blog is a great resource for anyone who feels overwhelmed by financial planning.",
                 256, 48, 25),
                ("Reliance to invest in AI-driven supply chain innovation",
                 "This article really breaks down the key principles of financial management.",
                 "Clear explanation and practical insights on how AI can reshape operations.",
                 340, 76, 45),
                ("Infosys launches new digital transformation initiative",
                 "This article really breaks down the key principles of financial management.",
                 "Very informative post on digital transformation trends.",
                 98, 12, 8),
                ("Wipro secures contract for global cloud migration project",
                 "Wipro will assist a Fortune 500 company with its large-scale migration to cloud infrastructure.",
                 "A solid overview of enterprise cloud adoption.",
                 178, 22, 14),
                ("HCL Tech partners with global bank for AI-powered solutions",
                 "HCL Tech announced a multi-year partnership to integrate AI-driven banking solutions.",
                 "AI adoption in banking explained very well.",
                 210, 39, 19),
                ("L&T Construction bags mega order in Middle East",
                 "Larsen & Toubro won a major infrastructure order in the Gulf region.",
                 "Great coverage of international infrastructure projects.",
                 301, 54, 33),
                ("Tech Mahindra launches blockchain solution for supply chains",
                 "Tech Mahindra introduced a blockchain solution to improve supply chain transparency.",
                 "Blockchain use cases explained in a simple way.",
                 147, 18, 10),
            };

            db.ContentItems.AddRange(items.Select((b, i) => new ContentItem {
                Id            = Guid.NewGuid(),
                TenantId      = tenant.Id,
                ContentType   = contentType,
                Title         = b.Item1,
                Description   = b.Item2,
                ImageUrl      = $"https://picsum.photos/seed/blog{i + 1}/400/300",
                AuthorName    = "Emily Brown",
                AuthorImage   = "/assets/images/profile.png",
                Likes         = b.Item4,
                CommentsCount = b.Item5,
                Shares        = b.Item6,
                PublishedAt   = new DateOnly(2025, 8, 10),
                ExtraJson     = $"{{\"rcomments\":\"{b.Item3}\"}}"
            }));

            await db.SaveChangesAsync();
            Console.WriteLine($"[Seeder] Content seeded for '{tenant.Slug}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seeder] Content seed failed for '{tenant.Slug}': {ex.Message}");
        }
    }
}


