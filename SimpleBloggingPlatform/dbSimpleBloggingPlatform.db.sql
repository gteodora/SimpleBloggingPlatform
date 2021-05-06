BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "post" (
	"slug"	TEXT NOT NULL,
	"title"	TEXT NOT NULL,
	"description"	TEXT NOT NULL,
	"body"	TEXT NOT NULL,
	"created_at"	TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"updated_at"	TEXT,
	PRIMARY KEY("slug")
);
CREATE TABLE IF NOT EXISTS "tag" (
	"id"	INTEGER NOT NULL DEFAULT 1,
	"name"	TEXT NOT NULL UNIQUE,
	PRIMARY KEY("id" AUTOINCREMENT)
);
CREATE TABLE IF NOT EXISTS "post_tag" (
	"tag_id"	INTEGER,
	"post_slug"	TEXT,
	FOREIGN KEY("tag_id") REFERENCES "tag"("id") ON UPDATE SET NULL ON DELETE SET NULL,
	FOREIGN KEY("post_slug") REFERENCES "post"("slug") ON UPDATE SET NULL ON DELETE SET NULL,
	PRIMARY KEY("tag_id","post_slug")
);
INSERT INTO "post" VALUES ('augmented-reality-ios-application-2','Augmented Reality iOS Application 2','Rubicon Software Development and Gazzda furniture are proud to launch an augmented reality app.','The app is simple to use, and will help you decide on your best furniture fit.','2021-05-05T19:56:39.509Z',NULL);
INSERT INTO "tag" VALUES (1,'iOS');
INSERT INTO "tag" VALUES (2,'Android');

INSERT INTO "post_tag" VALUES (1,'augmented-reality-ios-application-2');

COMMIT;
