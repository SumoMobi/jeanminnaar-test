using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace WebApi1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c => 
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "User API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Email = "jean.minnaar@bannerhealth.com",
                        Name = "Jean Minnaar",
                        Url = "https://www.jeanminnaar.com"
                    },
                    Description = "API to manage user information",
                    License = new License
                    {
                        Name = "minnaar license",
                        Url = "https://www.jeanminnaar.com/license"
                    },
                    TermsOfService = "https://www.jeanminnaar.com/terms"
                }); 
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            //Enable middleware to server swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
