// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-watch');

    grunt.initConfig({
        //uglify: {
        //    my_target:{
        //        files: {'wwwroot/app.js':['Scripts/app.js', 'Scripts/**/*.js']}
        //    }
        //},
        
        concat: {
            my_target:{
                files: {'wwwroot/app.js':['Scripts/app.js', 'Scripts/**/*.js']}
            }
        },

        watch: {
            scripts:{
                files: ['Scripts/**/*.js'],
                tasks: ['concat']
            }
        },

        bower: {
            install: {
                options: {
                    targetDir: "wwwroot/lib",
                    layout: "byComponent",
                    cleanTargetDir: false
                }
            }
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
    grunt.registerTask("default", ["bower:install", 'concat', 'watch']);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
};