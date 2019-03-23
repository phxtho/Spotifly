CREATE TABLE playlist (
id BIGINT NOT NULL AUTO_INCREMENT,
owner_id BIGINT NOT NULL,
name VARCHAR(128) NOT NULL,
description VARCHAR(768),
spotify_href VARCHAR(256),
public BOOLEAN NOT NULL,
CONSTRAINT PK_PLAYLIST_ID PRIMARY KEY(id),
CONSTRAINT FOREIGN KEY(owner_id) REFERENCES `user`(id)
);

CREATE TABLE playlist_track (
playlist_id BIGINT NOT NULL,
track_id BIGINT NOT NULL,
CONSTRAINT PRIMARY KEY(playlist_id, track_id),
CONSTRAINT FOREIGN KEY(playlist_id) REFERENCES playlist(id),
CONSTRAINT FOREIGN KEY(track_id) REFERENCES track(id)
);