CREATE TABLE public.books
(
    id character varying(32) NOT NULL,
    title character varying(32) NOT NULL,
    author character varying(32) NOT NULL,
    CONSTRAINT id PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public.books
    OWNER to postgres;

INSERT INTO public.books (id,title, author) VALUES ('1','たったひとつの冴えたやりかた','ジェイムズ・ティプトリー・ジュニア');
INSERT INTO public.books (id,title, author) VALUES ('2','アンドロイドは電気羊の夢を見るか?','フィリップ・K・ディック');
INSERT INTO public.books (id,title, author) VALUES ('3','夏への扉','ロバート・A. ハインライン');
INSERT INTO public.books (id,title, author) VALUES ('4','幼年期の終り','アーサー C クラーク');
INSERT INTO public.books (id,title, author) VALUES ('5','われはロボット','アイザック・アシモフ');
