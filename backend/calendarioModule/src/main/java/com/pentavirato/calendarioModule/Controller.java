package com.pentavirato.calendarioModule;

import com.pentavirato.calendarioModule.modelo.Calendar;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/calendar")
public class Controller {
	private Calendar calendarService;


    @GetMapping("/hola")
	public String getHello() {
		return "Hola";
	}

	@GetMapping("/abel")
	public String getAbel() {
		return "Abel es un pendejo";
	}
}
