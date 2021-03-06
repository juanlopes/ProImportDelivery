/* -| 17/04/2020 |--------------------------------------------------------------
*    criação de script para a implementação do ProImport
*    Desenvolvedor Responsável: Juan Matheus Lopes
*/

-- Deleta a tabela existente (caso exista)
--DROP TABLE PROIMPORT02;

-- Cria a tabela para a utilização do ProImport
CREATE TABLE PROIMPORT02 (
       COD NUMERIC(6,0),
	   EST NUMERIC(5,0)
);

CREATE TABLE PROIMPORT03 (
	   p2cod numeric(7,0) unique not null,
       cod varchar(14) unique not null, 
	   dsc varchar(60) not null, 
	   pmc numeric(14,2), 
	   sec varchar(60), 
	   est numeric(6,0), 
	   lim numeric(6,0), 
	   dti date, 
	   dtf date
);


/* For Sysmo S1 Users only */
/*
-- Insere os dados na tabela Criada Anteriormente
insert into proimport02 (cod, est)
select i1.pro, 300
from gceitm01 i1
where op1 = 'S' and
i1.dtc between current_date - interval '3 month' and current_date
group by i1.pro
order by sum(i1.qnt) desc
--> Alterar somente a quantidade de produtos abaixo
limit 500;
*/

/*
--select * from proimport02 order by 1 asc;
*/