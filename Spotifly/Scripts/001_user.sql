CREATE TABLE user (
id BIGINT NOT NULL AUTO_INCREMENT,
name VARCHAR(255) NOT NULL,
email VARCHAR(255) NOT NULL,
password VARCHAR(128) NOT NULL,
date_created DATETIME DEFAULT NOW() NOT NULL,
CONSTRAINT PK_USER_ID PRIMARY KEY(id),
CONSTRAINT UQ_USER_EMAIL UNIQUE(email)
);

CREATE TABLE user_token (
    id BIGINT NOT NULL,
    access_token VARCHAR(255),
    refresh_token VARCHAR(255),
    date_created DATETIME,
    CONSTRAINT PK_USER_TOKEN_ID PRIMARY KEY(id),
    CONSTRAINT FK_USER_TOKEN_ID_USER_ID FOREIGN KEY (id) REFERENCES `user`(id)
);

CREATE TABLE friend_request (
    sender_id BIGINT NOT NULL,
    receiver_id BIGINT NOT NULL,
    date_created DATETIME DEFAULT NOW() NOT NULL,
    CONSTRAINT PK_FRIEND_REQUEST_ID PRIMARY KEY(sender_id, receiver_id),
    CONSTRAINT FK_FRIENDE_REQUEST_SENDER_ID_USER_ID FOREIGN KEY (sender_id) REFERENCES `user`(id),
    CONSTRAINT FK_FRIENDE_REQUEST_RECEIVER_ID_USER_ID FOREIGN KEY (receiver_id) REFERENCES `user`(id)
);

CREATE TABLE friend (
user1_id BIGINT NOT NULL,
user2_id BIGINT NOT NULL,
date_created DATETIME DEFAULT NOW() NOT NULL,
CONSTRAINT PK_FRIEND_USER1_USER2 PRIMARY KEY(user1_id, user2_id),
CONSTRAINT FK_FRIEND_USER1_ID FOREIGN KEY (user1_id) REFERENCES `user`(id),
CONSTRAINT FK_FRIEND_USER2_ID FOREIGN KEY (user2_id) REFERENCES `user`(id),
CONSTRAINT CHK_FRIEND_USER1_USER2 CHECK (user1_id > user2_id)
);