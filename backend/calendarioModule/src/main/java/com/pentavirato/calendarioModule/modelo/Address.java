package com.pentavirato.calendarioModule.modelo;

import jakarta.persistence.*;

@Embeddable
public class Address {
    private String principalStreet;
    private String secondaryStreet;
    private String reference;

    public Address() {
    }

    public Address(String principalStreet, String secondaryStreet, String reference) {
        this.principalStreet = principalStreet;
        this.secondaryStreet = secondaryStreet;
        this.reference = reference;
    }

    public String getPrincipalStreet() {
        return principalStreet;
    }

    public void setPrincipalStreet(String principalStreet) {
        this.principalStreet = principalStreet;
    }

    public String getSecondaryStreet() {
        return secondaryStreet;
    }

    public void setSecondaryStreet(String secondaryStreet) {
        this.secondaryStreet = secondaryStreet;
    }

    public String getReference() {
        return reference;
    }

    public void setReference(String reference) {
        this.reference = reference;
    }
}
