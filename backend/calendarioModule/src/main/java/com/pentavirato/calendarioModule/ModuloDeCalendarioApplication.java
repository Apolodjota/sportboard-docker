package com.pentavirato.calendarioModule;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;

@SpringBootApplication
public class ModuloDeCalendarioApplication {

	public static void main(String[] args) {
		SpringApplication.run(ModuloDeCalendarioApplication.class, args);
	}
}
