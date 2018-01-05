const
    gulp         = require('gulp'),
    sass         = require('gulp-sass'),
    browserSync  = require('browser-sync').create(),
    autoprefixer = require('gulp-autoprefixer'),
    cleanCSS     = require('gulp-clean-css'),
    sourcemaps   = require('gulp-sourcemaps'),
    cache        = require('gulp-cache'),
    imagemin     = require('gulp-imagemin'), // Подключаем библиотеку для работы с изображениями
    pngquant     = require('imagemin-pngquant'), // Подключаем библиотеку для работы с png
    uglify       = require('gulp-uglify'),
    browserify   = require('gulp-browserify'),
    rename       = require('gulp-rename'),
    del          = require('del'); // очистка папки

const srcDir = './src';
const buildDir = './build';

const path = {
    srcDir:   srcDir,
    buildDir: buildDir,

    html: {
        src:   srcDir + '/*.html',
        watch: srcDir + '/*.html',
        build: buildDir
    },
    css: {
        src:   srcDir + '/scss/**/*.scss',
        watch: srcDir + '/scss/**/*.scss',
        build: buildDir + '/css/'
    },
    js: {
        src:   srcDir + '/js/app.js',
        watch: srcDir + '/js/**/*.js',
        build: buildDir + '/js/'
    },
    img: {
        src:   srcDir + '/img/**/*.*',
        build: buildDir + '/img/'
    },
    fonts: {
        src:   srcDir + '/fonts/**/*.*',
        build: buildDir + '/fonts/'
    }
};

gulp.task('html', function () {
    gulp.src(path.html.src)
        .pipe(gulp.dest(path.html.build));
});

gulp.task('scss', function(){
    gulp.src(path.css.src)
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer({
            browsers: ['> 0.1%'],
            cascade: false
        }))
        // .pipe(cleanCSS())                 //минифицирует файл
        .pipe(sourcemaps.write('.'))         //sourcemaps
        .pipe(gulp.dest(path.css.build));
});

gulp.task('js', function() {
    gulp.src(path.js.src)
        .pipe(browserify({
            insertGlobals : true,
            debug : true
        }))
        .pipe(rename('bundle.js'))
        // .pipe(uglify())                      // Минимизировать весь js (на выбор)
        .pipe(gulp.dest(path.js.build))
    // .pipe(browserSync.reload({stream: true}));
});

gulp.task('img', function() {
    return gulp.src(path.img.src) // Берем все изображения из app
        .pipe(cache(imagemin({  // Сжимаем их с наилучшими настройками с учетом кеширования
            interlaced: true,
            progressive: true,
            svgoPlugins: [{removeViewBox: false}],
            use: [pngquant()]
        })))
        .pipe(gulp.dest(path.img.build)); // Выгружаем на продакшен
});

gulp.task('fonts', function () {
    gulp.src(path.fonts.src)
        .pipe(gulp.dest(path.fonts.build));
});

// удаление папки назначения
gulp.task('cleanDist', function () {
    return del.sync(path.buildDir)
});

gulp.task('browserSync', function(){
    browserSync.init({
        server: {
            baseDir: path.buildDir
        },
        notify: false
    });
});

// отслеживание за изменениями в файле
gulp.task('watch', function(){
    gulp.watch(path.html.watch, ['html', browserSync.reload]);
    gulp.watch(path.css.watch, ['scss', browserSync.reload]);
    gulp.watch(path.js.watch, ['js', browserSync.reload]);
    gulp.watch(path.img.src, ['img', browserSync.reload]);
    gulp.watch(path.fonts.src, ['fonts', browserSync.reload]);
});

gulp.task('build', ['cleanDist', 'html', 'img', 'scss', 'js', 'fonts']);

gulp.task('default', ['build', 'browserSync', 'watch']);